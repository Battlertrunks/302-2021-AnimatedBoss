using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateMachine : MonoBehaviour {

    static class States {
        public class State {

            protected BossStateMachine bossState;

            virtual public State Update() {
                return null;
            }

            virtual public void OnStart(BossStateMachine bossState) {
                this.bossState = bossState;
            }

            virtual public void OnEnd() {

            }
        }

        ///////// Child Classes (Boss States):
        ///

        public class Idle : State {

            public override State Update() {
                
                return null;
            }
        }

        public class Walk_Patrol : State {

            public override State Update() {

                return null;
            }
        }

        public class Attack : State {

            public override State Update() {

                return null;
            }
        }

        public class deathAnim : State {

            public override State Update() {

                return null;
            }
        }


    }

    void Start() {
        
    }


    void Update() {
        
    }
}
