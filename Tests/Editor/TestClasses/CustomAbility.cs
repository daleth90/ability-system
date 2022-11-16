using UnityEngine;

namespace Physalia.AbilityFramework.Tests
{
    public static class CustomAbility
    {
        public static string HELLO_WORLD
        {
            get
            {
                return ReadAbilityFile("HelloWorld");
            }
        }

        public static string NORAML_ATTACK
        {
            get
            {
                return ReadAbilityFile("NormalAttack");
            }
        }

        public static string NORAML_ATTACK_SELECTION
        {
            get
            {
                return ReadAbilityFile("NormalAttackSelection");
            }
        }

        public static string NORMAL_ATTACK_5_TIMES
        {
            get
            {
                return ReadAbilityFile("NormalAttack5Times");
            }
        }

        public static string ATTACK_DECREASE
        {
            get
            {
                return ReadAbilityFile("AttackDecrease");
            }
        }

        public static string ATTACK_DOUBLE
        {
            get
            {
                return ReadAbilityFile("AttackDouble");
            }
        }

        public static string ATTACK_UP_WHEN_LOW_HEALTH
        {
            get
            {
                return ReadAbilityFile("AttackUpWhenLowHealth");
            }
        }

        public static string ATTACK_DOUBLE_WHEN_DAMAGED
        {
            get
            {
                return ReadAbilityFile("AttackDoubleWhenDamaged");
            }
        }

        public static string COUNTER_ATTACK
        {
            get
            {
                return ReadAbilityFile("CounterAttack");
            }
        }

        private static string ReadAbilityFile(string fileName)
        {
            TextAsset asset = Resources.Load<TextAsset>(fileName);
            if (asset == null)
            {
                return "";
            }

            return asset.text;
        }
    }
}
