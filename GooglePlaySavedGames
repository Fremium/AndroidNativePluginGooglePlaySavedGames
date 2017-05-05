using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class GooglePlaySavedGames : MonoBehaviour 
{
	private Text textPlayerName;
	private AndroidMessage androidMessage;
	private bool onSocialSharingIsPosted = false;
	private bool timerOnSocialSharingIsTurnedOn = false;
	private float waitTimeAfterReturnFromPauseState = 0.5f;


	#region UNITY PRIVATE METHODS
	private void Start () 
	{
		textPlayerName = (Text)GameObject.Find("Text_PlayerName").GetComponent("Text");
		GooglePlayConnection.ActionPlayerConnected +=  OnPlayerConnected;
		GooglePlayConnection.ActionPlayerDisconnected += OnPlayerDisconnected;		
		GooglePlayConnection.ActionConnectionResultReceived += OnConnectionResult;		
		GooglePlaySavedGamesManager.ActionGameSaveLoaded += ActionGameSaveLoaded;
		GooglePlaySavedGamesManager.ActionConflict += ActionConflict;
	}

	private void OnDestroy() 
	{
		GooglePlayConnection.ActionPlayerConnected -=  OnPlayerConnected;
		GooglePlayConnection.ActionPlayerDisconnected -= OnPlayerDisconnected;		
		GooglePlayConnection.ActionConnectionResultReceived -= OnConnectionResult;
		GooglePlaySavedGamesManager.ActionGameSaveLoaded -= ActionGameSaveLoaded;
		GooglePlaySavedGamesManager.ActionConflict -= ActionConflict;
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!pauseStatus)
		{
			if (onSocialSharingIsPosted)
			{
				onSocialSharingIsPosted = false;
				//SaveGame();
				timerOnSocialSharingIsTurnedOn = true;
				waitTimeAfterReturnFromPauseState = 0;
			}
		}
	}

	// Update() is used since SaveGame() call from OnApplicationPause() causes app to freeze when calling several times RateUs()
	private void Update()
	{
		if (timerOnSocialSharingIsTurnedOn)
		{
			waitTimeAfterReturnFromPauseState += Time.deltaTime;
			if (waitTimeAfterReturnFromPauseState > 0.5f)
			{
				timerOnSocialSharingIsTurnedOn = false;
				SaveGame();
			}
		}
	}

	#endregion

	#region UNITY PUBLIC METHODS
	public void ConnectToGooglePlay()
	{
		if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
		{
			OnPlayerConnected();
		}
		else
		{
			GooglePlayConnection.Instance.Connect();
		}
	}
	
	public void SaveGame()
	{
		AndroidNativeUtility.ShowPreloader("\"Game\"", "Saving Game...\n\nPlease, wait!");
		try
		{
			StartCoroutine(MakeScreenshotAndSaveGameData());
		}
		catch (Exception e)
		{
			Debug.Log("SaveGame Exception: " + e.Message);
			AndroidNativeUtility.HidePreloader();
		}
	}
	
	public void LoadGame()
	{
		AndroidNativeUtility.ShowPreloader("\"Game\"", "Loading Game...\n\nPlease, wait!");
		GooglePlaySavedGamesManager.ActionAvailableGameSavesLoaded += ActionAvailableGameSavesLoaded;
		try
		{
			GooglePlaySavedGamesManager.Instance.LoadAvailableSavedGames();
		}
		catch (Exception e)
		{
			Debug.Log("LoadGame Exception: " + e.Message);
			AndroidNativeUtility.HidePreloader();
		}
	}
	
	public void RateUs()
	{
		androidMessage = AndroidMessage.Create("Rate \"Game\" game in \"Google Play\" ", "Rate our game in \"Google Play\".");
		androidMessage.ActionComplete += OnRateUsActionComplete;
	}
	#endregion

	#region EVENT HANDLERS
	private void OnRateUsActionComplete(AndroidDialogResult result)
	{
		androidMessage.ActionComplete -= OnRateUsActionComplete;
		if (result == AndroidDialogResult.CLOSED)
		{
			onSocialSharingIsPosted = true;
			Application.OpenURL("market://details?id=com.rovio.angrybirds");
		}
	}

	private void OnPlayerConnected() 
	{
		textPlayerName.text = GooglePlayManager.Instance.player.name;
	}

	private void OnPlayerDisconnected()
	{
		textPlayerName.text = "Player Name";
	}

	private void OnConnectionResult(GooglePlayConnectionResult result) 
	{	
		Debug.Log("ConnectionResult: " + result.code.ToString());
	}

	private void ActionGameSaveLoaded (GP_SpanshotLoadResult result) 
	{		
		Debug.Log("ActionGameSaveLoaded: " + result.Message);
		if(result.IsSucceeded) 
		{			
			Debug.Log("Snapshot.Title: " 					+ result.Snapshot.meta.Title);
			Debug.Log("Snapshot.Description: " 				+ result.Snapshot.meta.Description);
			Debug.Log("Snapshot.CoverImageUrl): " 			+ result.Snapshot.meta.CoverImageUrl);
			Debug.Log("Snapshot.LastModifiedTimestamp: " 	+ result.Snapshot.meta.LastModifiedTimestamp);
			
			Debug.Log("Snapshot.stringData: " 				+ result.Snapshot.stringData);
			Debug.Log("Snapshot.bytes.Length: " 			+ result.Snapshot.bytes.Length);
			
			AndroidNativeUtility.HidePreloader();
			AndroidMessage.Create("Snapshot Loaded", "Data: " + result.Snapshot.stringData);
		} 
	}

	private void ActionAvailableGameSavesLoaded(GooglePlayResult res)
	{
		GooglePlaySavedGamesManager.ActionAvailableGameSavesLoaded -= ActionAvailableGameSavesLoaded;
		if (res.IsSucceeded)
		{
			try
			{
				foreach (GP_SnapshotMeta meta in GooglePlaySavedGamesManager.Instance.AvailableGameSaves)
				{
					Debug.Log("Meta.Title: " + meta.Title);
					Debug.Log("Meta.Description: " + meta.Description);
					Debug.Log("Meta.CoverImageUrl): " + meta.CoverImageUrl);
					Debug.Log("Meta.LastModifiedTimestamp: " + meta.LastModifiedTimestamp);
					Debug.Log("Meta.TotalPlayedTime" + meta.TotalPlayedTime);
				}
				
				if (GooglePlaySavedGamesManager.Instance.AvailableGameSaves.Count > 0)
				{
					GP_SnapshotMeta s = GooglePlaySavedGamesManager.Instance.AvailableGameSaves[0];
					GooglePlaySavedGamesManager.Instance.LoadSpanshotByName(s.Title);
				}
			}
			catch (Exception e)
			{
				Debug.Log("ActionAvailableGameSavesLoaded Exception: " + e.Message);
				AndroidNativeUtility.HidePreloader();
			}
		}
		else
		{
			AndroidNativeUtility.HidePreloader();
			AndroidMessage.Create("\nGame\n", "Failed to load game data from Cloud in Internet.\n\nPlease, check your Internet connection and try again.");
		}
	}

	private void ActionGameSaveResult (GP_SpanshotLoadResult result) 
	{
		GooglePlaySavedGamesManager.ActionGameSaveResult -= ActionGameSaveResult;
		Debug.Log("ActionGameSaveResult: " + result.Message);
		
		AndroidNativeUtility.HidePreloader();
		if(result.IsSucceeded) 
		{
			AndroidMessage.Create("Game saved", "Data: " + result.Snapshot.stringData);
		} 
		else 
		{
			Debug.Log("ActionGameSaveResult Error : Games Save Failed");
			AndroidMessage.Create("\"City Car Run\" Error", "Game Data Save Failed");
		}
	}

	private void ActionConflict (GP_SnapshotConflict result) 
	{	
		Debug.Log("Conflict Detected: ");
		
		GP_Snapshot snapshot = result.Snapshot;
		GP_Snapshot conflictSnapshot = result.ConflictingSnapshot;
		
		// Resolve between conflicts by selecting the newest of the conflicting snapshots.
		GP_Snapshot mResolvedSnapshot = snapshot;
		
		if (snapshot.meta.LastModifiedTimestamp < conflictSnapshot.meta.LastModifiedTimestamp) {
			mResolvedSnapshot = conflictSnapshot;
		}
		
		result.Resolve(mResolvedSnapshot);
	}
	#endregion

	#region HELPER METHODS
	private IEnumerator MakeScreenshotAndSaveGameData() 
	{
		yield return new WaitForEndOfFrame();
		
		// Create a texture the size of the screen, RGB24 format
		int width = Screen.width;
		int height = Screen.height;
		Texture2D Screenshot = new Texture2D( width, height, TextureFormat.RGB24, false );
		
		// Read screen contents into the texture
		Screenshot.ReadPixels( new Rect(0, 0, width, height), 0, 0 );
		Screenshot.Apply();

		long TotalPlayedTime = 20000;
		//string currentSaveName =  "snapshotTemp-" + Random.Range(1, 281).ToString();
		string currentSaveName =  "TestingSameName";
		string description  = "Modified data at: " + System.DateTime.Now.ToString("MM/dd/yyyy H:mm:ss");
		
		GooglePlaySavedGamesManager.ActionGameSaveResult += ActionGameSaveResult;
		GooglePlaySavedGamesManager.Instance.CreateNewSnapshot(currentSaveName,
		                                                       description,
		                                                       Screenshot,
		                                                       "some save data, for example you can use JSON or byte array " + UnityEngine.Random.Range(1, 10000).ToString(),
		                                                       TotalPlayedTime);		
		Destroy(Screenshot);
	}
	#endregion
}
