using System;
using System.Collections;
using Character.Monster;
using Defines;
using Keiwando.BigInteger;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

public class SkillSystem : AttackSystem
{
    [Header("지정 필요")]
    public string skillName;
    public SkillSystem[] subSkill;
    
    public ActiveSkillData activeSkillData { get; private set; }
    private int colliderDataIndex;
    private Transform targetTransform = null;
    private Transform master;
    private bool isMaster;
    private bool isEnd;
    private bool isTargeting;

    public void InitData()
    {
        foreach (var sub in subSkill)
        {
            sub.InitData();
        }
    }

    public void InitSkillSystem<T>(PlayerData data, T skill) where T : ActiveSkillData
    {
        activeSkillData = skill;
        InitAttackSystem(data);

        foreach (var sub in subSkill)
        {
            sub.InitSkillSystem(data, skill);
        }

        master = data.transform;
    }

    public override void InitAttackSystem(PlayerData data)
    {
        status = data.status;
        targetLayerMask = data.targetLayerMask;
        targetTag = data.targetTag;

        controller = data.controller;
        spriteController = data.spriteController;

        if (attackColliders != null)
        {
            attackDatas = new AttackData[attackColliders.Length];
            for (int i = 0; i < attackColliders.Length; ++i)
            {
                attackColliders[i].InitAttackCollider(data);
                attackDatas[i] = new AttackData(data, activeSkillData.fixedInfo.colliderInfo[i].knockback, activeSkillData.Multiplier+status.currentSkillDamage, activeSkillData.maxAttackCount,
                    activeSkillData.fixedInfo.isContinuous, activeSkillData.tickUnitTime);
            }

            if (attackRangeSystem != null)
                attackRangeSystem.InitAttackRangeSystem(this);
        }
    }

    public virtual void StartSkill()
    {
        StartVarSetting(true);
        Vector3 direction = Vector3.zero; 

        if (activeSkillData.fixedInfo.isFollowing)
        {
            //SetPositionAndRotationTargetToPlayer(target);
        }
        else
        {
            transform.position = PlayerManager.instance.player.transform.position;
            // Debug.Log($"skill direction {direction.x}.{direction.y}.{direction.z}");
            direction = Vector3.left * PlayerManager.instance.player.spriteController.horizontalDirection;
            var dir = GetLeftOrRight(direction);
            // Debug.Log($"skill dir {dir.x}.{dir.y}.{dir.z}");
            var local = transform.localScale;
            transform.localScale = new Vector3(Mathf.Abs(local.x) * dir.x, local.y * dir.y, local.z * dir.z);
        }

        StartCoroutine(SingleMeleeSkill());
    }

    private Vector3 GetLeftOrRight(Vector3 direction)
    {
        var dec = Vector2.Dot(direction, Vector2.right);
        if (dec < 0)
        {
            return new Vector3(1, 1, 1);
        }
        else
        {
            return new Vector3(-1, 1, 1);
        }
    }

    public virtual void StartSkill(Transform transform)
    {
        ((Component)this).transform.position = transform.position;
        targetTransform = transform;
        StartVarSetting(false);

        StartCoroutine(MultiTargetingSkill());
    }

    public virtual bool StartSkill(Transform[] transforms)
    {
        StartVarSetting(true);
        int enemy = 0;
        bool isPerformed = false;
        for (int i = 0; i < subSkill.Length && i < transforms.Length; ++i)
        {
            Transform target = null;

            while (enemy < transforms.Length)
            {
                if (Utils.Vector.BoxDistance(transforms[enemy].position, master.position, 1, 2) < activeSkillData.attackDistance)
                {
                    target = transforms[enemy];
                    ++enemy;
                    break;
                }
                ++enemy;
            }

            // TODO : 분기
            if (activeSkillData.fixedInfo.isRotationNeeded)
            {
                SetPositionAndRotationTargetToPlayer(target, subSkill[i].transform);
            }
            else
            {
                if (targetTransform != null)
                {
                    if (!targetTransform.gameObject.activeInHierarchy)
                    {
                        targetTransform = null;
                        isTargeting = false;
                    }
                    else
                        transform.position = targetTransform.position;
                }
                else
                    isTargeting = false;
            }
            
            if (target != null)
            {
                // 서브 스킬
                subSkill[i].StartSkill(target);
                isPerformed = true;
            }
            else
                break;
        }

        if (isPerformed)
        {
            // 본 스킬
            StartCoroutine(MultiTargetingSkill());
            return true;
        }
        else
        {
            gameObject.SetActive(false);
            targetTransform = null;
            StopSubSkill();
            return false;
        }
    }

    private void SetPositionAndRotationTargetToPlayer(Transform target, Transform skillTransform)
    {
        if (target == null)
            return;

        skillTransform.position = target.position;
        Vector3 direction = PlayerManager.instance.player.transform.position - target.position;
        Vector3 normalizedDirection = direction.normalized;
        float angle = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg - 90f;
        
        skillTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void StartVarSetting(bool isMaster)
    {
        // elapsedTime = .0f;
        colliderDataIndex = 0;
        gameObject.SetActive(true);
        // count = 0;
        this.isMaster = isMaster;
        isEnd = false;
        isTargeting = activeSkillData.fixedInfo.isFollowing;
    }

    public void StopSubSkill()
    {
        if (isMaster)
        {
            foreach (var atkcol in attackColliders)
            {
                atkcol.ClearTickList();
            }
            foreach (var skill in subSkill)
            {
                skill.StopSubSkill();
            }
        }
        else
        {
            gameObject.SetActive(false);
            targetTransform = null;
            foreach (var atkcol in attackColliders)
            {
                atkcol.ClearTickList();
            }
        }
    }

    private IEnumerator MultiTargetingSkill()
    {
        float elapsedTime = .0f;
        while (elapsedTime < activeSkillData.skillFullTime)
        {
            elapsedTime += Time.deltaTime;

            if (!isEnd && !isMaster)
            {
                while (attackColliders.Length > colliderDataIndex &&
                       activeSkillData.fixedInfo.colliderInfo[colliderDataIndex].startTime / status.currentAttackSpeed < elapsedTime)
                {
                    var atkcol = activeSkillData.fixedInfo.colliderInfo[colliderDataIndex];
                    AttackEvent(atkcol.offset, atkcol.type, atkcol.size, colliderDataIndex, atkcol.knockback,
                        atkcol.duration);
                    ++colliderDataIndex;
                    if (colliderDataIndex >= activeSkillData.fixedInfo.colliderInfo.Length)
                    {
                        isEnd = true;
                        break;
                    }
                }
            }

            yield return null;
        }

        gameObject.SetActive(false);
        targetTransform = null;
        StopSubSkill();
    }

    private IEnumerator SingleMeleeSkill()
    {
        float elapsedTime = .0f;
        float total = .0f;

        ActiveSkillFixedInfo info = activeSkillData.fixedInfo;

        while (total < activeSkillData.skillFullTime)
        {
            elapsedTime += Time.deltaTime;
            total += Time.deltaTime;
            if (!isEnd)
            {
                while (info.colliderInfo.Length > colliderDataIndex && info.colliderInfo[colliderDataIndex].startTime / status.currentAttackSpeed < elapsedTime)
                {
                    var atkcol = info.colliderInfo[colliderDataIndex];
                    AttackEvent(atkcol.offset, atkcol.type, atkcol.size, colliderDataIndex, atkcol.knockback, atkcol.duration);
                    ++colliderDataIndex;
                    if (colliderDataIndex >= info.colliderInfo.Length)
                    {
                        if (info.isRepeat)
                        {
                            colliderDataIndex = 0;
                            elapsedTime = - atkcol.duration;
                        }
                        else
                            isEnd = true;
                        break;
                    }
                }
            }
            
            yield return null;
        }
        
        targetTransform = null;
        StopSubSkill();
        gameObject.SetActive(false);
    }
}