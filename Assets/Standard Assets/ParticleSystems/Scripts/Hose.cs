using System;
using UnityEngine;


namespace UnityStandardAssets.Effects
{
    public class Hose : MonoBehaviour
    {
        public float maxPower = 0;
        public float minPower = 0;
        public float changeSpeed = 0;
        public ParticleSystem[] hoseWaterSystems;
        public Renderer systemRenderer;

        private float m_Power;


        // Update is called once per frame
        private void Update()
        {
            m_Power = maxPower;//Mathf.Lerp(m_Power, Input.GetMouseButton(0) ? maxPower : minPower, Time.deltaTime*changeSpeed);

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                systemRenderer.enabled = !systemRenderer.enabled;
            }

            foreach (var system in hoseWaterSystems)
            {
                system.startSpeed = m_Power;
                var emission = system.emission;
                emission.enabled = (m_Power > minPower*1.1f);
            }
        }
    }
}
