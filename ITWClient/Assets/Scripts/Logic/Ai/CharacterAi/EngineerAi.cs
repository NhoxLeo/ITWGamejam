﻿using UnityEngine;
using System.Collections;

namespace Ai
{
    public class EngineerAi : ICharacterAi
    {
        protected override void CreateBehaviours()
        {
            base.CreateBehaviours();
            Behaviours[AiState.Skill] = new EngineerSkillBehaviour(this);
        }
    }
}