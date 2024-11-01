using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using UnityEngine;
using Unity.Services.Leaderboards;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using System;
using UnityEngine.TextCore.Text;
using Unity.Services.Leaderboards.Exceptions;
using Newtonsoft.Json;
using UnityEngine.SocialPlatforms.Impl;
using Unity.Services.Leaderboards.Models;
using TMPro;
using Unity.VisualScripting;

public class LeaderboardManager : MonoBehaviour
{
    public Transform LeaderboardContent;
    public Transform LeaderboardItemPrefab;
    public GameObject MainLeaderboard;

    private string NormalModeLeaderboardID = "Mergallies_Leaderboard";
    private bool isInitialized = false;
    
    // Start is called before the first frame update
    async void Start()
    {
        await InitializeServices();
    }

    // Update is called once per frame
    void Update()
    {
        // UpdateLeaderboard();
    }
    private async Task InitializeServices()
    {
        try
        {
            if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
            {
                await UnityServices.InitializeAsync();
            }

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            isInitialized = true;
            Debug.Log($"Services initialized. PlayerID: {AuthenticationService.Instance.PlayerId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize Unity Services: {e.Message}");
            isInitialized = false;
        }
    }
    public class NormalModeMetadata
    {
        public string DisplayName;
        public string MoveCount;
    }
    public async void UpdateLeaderboard(){
        if(MainLeaderboard.activeSelf){
            LeaderboardScoresPage leaderboardScoresPage = await LeaderboardsService.Instance.GetScoresAsync(NormalModeLeaderboardID,new GetScoresOptions
            {
                IncludeMetadata = true // Set to true to include metadata in the response
            });

            foreach(Transform t in LeaderboardContent){
                Destroy(t.gameObject);
            }

            foreach(Unity.Services.Leaderboards.Models.LeaderboardEntry entry in leaderboardScoresPage.Results){
                NormalModeMetadata metadata = JsonConvert.DeserializeObject<NormalModeMetadata>(entry.Metadata.ToString());
                double score = entry.Score; // Assuming score is in seconds
                // Calculate minutes, seconds, and milliseconds
                int minutes = (int)(score / 60);
                int seconds = (int)(score % 60);
                int milliseconds = (int)((score - (int)score) * 1000)/10; // Get milliseconds

                // Format the time as mm:ss.ms
                string formattedTime = string.Format("{0:D2}:{1:D2}.{2:D2}", minutes, seconds, milliseconds);

                Transform leaderboardItem = Instantiate(LeaderboardItemPrefab,LeaderboardContent);
                leaderboardItem.GetChild(0).GetComponent<TextMeshProUGUI>().text = "#" + (entry.Rank +1);
                leaderboardItem.GetChild(1).GetComponent<TextMeshProUGUI>().text = metadata.DisplayName;
                leaderboardItem.GetChild(2).GetComponent<TextMeshProUGUI>().text = formattedTime;
                leaderboardItem.GetChild(3).GetComponent<TextMeshProUGUI>().text = metadata.MoveCount;
            }
            Debug.Log("Leaderboard Updated");
        }
        await Task.Delay(1000);
    }
    public class LeaderboardEntry
    {
        public string PlayerName;
        public float Time;
        public int MoveCount;
    }
    public async void SubmitScore(string playerName, float time, int moveCount)
    {
        try
        {
            // Change anonymous ID first
            bool idChangeSuccess = await ChangeAnonymousUserId();
            if (!idChangeSuccess)
            {
                Debug.LogError("Failed to change anonymous ID before submitting score");
                return;
            }

            var metadata = new Dictionary<string, string>
            {
                { "DisplayName", playerName },
                { "MoveCount", moveCount.ToString() },
            };

            var addPlayerScoreOptions = new AddPlayerScoreOptions
            {
                Metadata = metadata
            };

            var playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync(
                NormalModeLeaderboardID, 
                time,          
                addPlayerScoreOptions
            );

            Debug.Log($"Score submitted successfully! Entry: {JsonConvert.SerializeObject(playerEntry)}");
        }
        catch (LeaderboardsException ex)
        {
            Debug.LogError($"Leaderboard error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Unexpected error during score submission: {ex.Message}");
        }
    }
    public async Task<bool> ChangeAnonymousUserId()
    {
        try
        {
            // Ensure we're initialized first
            if (!isInitialized)
            {
                await InitializeServices();
            }

            // Sign out current user
            if (AuthenticationService.Instance.IsSignedIn)
            {
                AuthenticationService.Instance.SignOut(true);
            }

            // Sign in with new anonymous ID
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Successfully changed anonymous ID. New PlayerID: {AuthenticationService.Instance.PlayerId}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to change anonymous user ID: {e.Message}");
            return false;
        }
    }
}