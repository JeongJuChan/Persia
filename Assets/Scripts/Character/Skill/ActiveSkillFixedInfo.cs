using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "SO/ActiveSkillFixedInfo", fileName = "ActiveSkillFixedInfo")]
public class ActiveSkillFixedInfo : FixedInfo
{
    [field: Header("Animation")]
    [field: SerializeField] public EFsmState animType { get; private set; }
    [field: SerializeField] public string animParameter { get; private set; }
    [field: SerializeField] public float skillAnimTime { get; private set; }

    [field: Header("Attack")]
    [field: SerializeField] public ESkillAttackType attackType { get; private set; }
    [field: SerializeField] public bool isFollowing { get; private set; } = false;
    [field: SerializeField] public bool isContinuous { get; private set; } = false;
    [field: SerializeField] public bool isRepeat { get; private set; } = false;
    [field: SerializeField] public bool isRotationNeeded { get; private set; } = false;
    [field: SerializeField] public float attackDistance { get; private set; }
    [field: SerializeField] public AttackColliderInfo[] colliderInfo { get; private set; }
}
