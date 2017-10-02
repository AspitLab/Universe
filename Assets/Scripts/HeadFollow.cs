using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class HeadFollow : MonoBehaviour {

    private FullBodyBipedIK m_IK;

    [SerializeField]
    private Transform m_Head;

	
	void Start () {
        m_IK = GetComponent<FullBodyBipedIK>();
    }
	
	
	void LateUpdate () {
        m_IK.solver.leftFootEffector.position = m_Head.position;
        m_IK.UpdateSolverExternal();
    }
}
