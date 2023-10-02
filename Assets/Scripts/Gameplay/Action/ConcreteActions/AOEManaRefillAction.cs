using Unity.BossRoom.Gameplay.GameplayObjects;
using Unity.BossRoom.Gameplay.GameplayObjects.Character;
using UnityEngine;

namespace Unity.BossRoom.Gameplay.Actions
{
    /// <summary>
    /// Area-of-effect mana restore Action. Area of effect of the spell is centered on a point provided by the client.
    /// </summary>

    [CreateAssetMenu(menuName = "BossRoom/Actions/AOE Mana Refill Action")]
    public class AOEManaRefillAction : AOEAction
    {
        private float m_timeElapsedSinceLastRefill;

        public override bool OnStart(ServerCharacter serverCharacter)
        {
            m_timeElapsedSinceLastRefill = Config.SpellEffectInterval;

           return base.OnStart(serverCharacter);
        }

        public override bool OnUpdate(ServerCharacter clientCharacter)
        {
            m_timeElapsedSinceLastRefill += Time.deltaTime;
            
            if (TimeRunning >= Config.ExecTimeSeconds && m_timeElapsedSinceLastRefill >= Config.SpellEffectInterval)
            {
                PerformAoE(clientCharacter);
            }

            return ActionConclusion.Continue;
        }

        private void PerformAoE(ServerCharacter parent)
        {
            var targetLayer = LayerMask.GetMask("PCs");
            
            var colliders = Physics.OverlapSphere(m_Data.Position, Config.Radius, targetLayer);
            for (var i = 0; i < colliders.Length; i++)
            {
                var targets = colliders[i].GetComponent<ManaOwner>();
                if (targets != null)
                {
                    // actually refill mana
                    targets.ReceiveMP(Config.Amount);
                }
            }

            // reset timer since last spell cast
            m_timeElapsedSinceLastRefill = 0f;
        }
    }
}