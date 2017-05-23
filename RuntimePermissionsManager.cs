using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RuntimePermissionsManager : MonoBehaviour 
{
	void Start()
	{
		PermissionsManager.ActionPermissionsRequestCompleted += HandleActionPermissionsRequestCompleted;	
	}

	void HandleActionPermissionsRequestCompleted (AN_GrantPermissionsResult res) 
	{
		Debug.Log("HandleActionPermissionsRequestCompleted");

		// AN_MenifestPermission -> ???
		foreach(KeyValuePair<AN_MenifestPermission, AN_PermissionState> pair in res.RequestedPermissionsState)
		{
			Debug.Log(pair.Key.GetFullName() + " / " + pair.Value.ToString());
		}		
	}
}
