using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AddOns;

namespace Managers {
    [RequireComponent(typeof(DontDestroyOnLoad))]
    public sealed class SoundManager : Manager<SoundManager>
    {
        private StageSFXPlaying _stageSounds;
        public StageSFXPlaying StageSounds => _stageSounds;

        protected override void Awake()
        {
            base.Awake();
            _stageSounds = GetComponent<StageSFXPlaying>();
        }
    }
}
