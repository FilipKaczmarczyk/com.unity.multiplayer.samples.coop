using System;
using Unity.BossRoom.Gameplay.GameplayObjects;
using Unity.BossRoom.Gameplay.GameplayObjects.Character;
using UnityEngine;

namespace Unity.BossRoom.Gameplay.Actions
{
    /// <summary>
    /// Area-of-effect mana restore Action. Area of effect of the spell is centered on a point provided by the client.
    /// </summary>

    [CreateAssetMenu(menuName = "BossRoom/Actions/AOE Mana Refill Action")]
    public class AOEManaRefillAction : Action
    {
        private float m_timeElapsedSinceLastRefill;

        public override bool OnStart(ServerCharacter serverCharacter)
        {
            m_timeElapsedSinceLastRefill = Config.SpellEffectInterval;
            
            // broadcasting to all players including myself.
            // We don't know our actual targets for this spell over time, so the client can't use the TargetIds list (and we clear it out for clarity).
            // This means we are responsible for triggering reaction-anims ourselves, which we do in PerformAoe()
            Data.TargetIds = new ulong[0];
            serverCharacter.serverAnimationHandler.NetworkAnimator.SetTrigger(Config.Anim);
            serverCharacter.clientCharacter.RecvDoActionClientRPC(Data);
            return ActionConclusion.Continue;
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
            
            var colliders = Physics.OverlapSphere(parent.physicsWrapper.Transform.position, Config.Radius, targetLayer);
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
        
        public override bool OnStartClient(ClientCharacter clientCharacter)
        {
            base.OnStartClient(clientCharacter);
            var selfDestructedAoeSphere = Instantiate(Config.SelfDestructedAoeSphere, clientCharacter.serverCharacter.physicsWrapper.Transform.position, Quaternion.identity);
            selfDestructedAoeSphere.gameObject.transform.localScale = new Vector3(Config.Radius * 2, Config.Radius * 2, Config.Radius * 2);
            selfDestructedAoeSphere.SetDestroyTime(Config.DurationSeconds);
            selfDestructedAoeSphere.transform.SetParent(clientCharacter.serverCharacter.physicsWrapper.Transform);
            return ActionConclusion.Stop;
        }
        
        public override bool OnUpdateClient(ClientCharacter clientCharacter)
        {
            throw new Exception("This should not execute");
        }
    }
}