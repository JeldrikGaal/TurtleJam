using System;
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;

public class LeaderBoardManager : MonoBehaviour
{
   public static LeaderBoardManager Instance;

   public bool OnlineEnabled = true;
   
   public static event Action<string> PlayerLoggedIn;
   public static event Action<GetLeaderboardResult> OnHighestScoreRetrieved;
   public static event Action<List<PlayerLeaderboardEntry>> OnLeaderBoardRetrieved;
   public static event Action<GetLeaderboardAroundPlayerResult> OnLeaderBoardAroundPlayerRetrieved;
   
   private void Awake()
   {
      if (Instance != null && Instance != this)
      {
         Destroy(gameObject);
      }
      else
      {
         Instance = this;
         DontDestroyOnLoad(gameObject);
      }
   }

   public void Login(string username)
   {
      if (OnlineEnabled)
      {
         PlayFabManager.Instance.Login(username);
      }
      else
      {
         OfflineLeaderBoard.Instance.Login(username);
      }
   }

   public string GetUserName()
   {
      if (OnlineEnabled)
      {
         return PlayFabManager.Instance.GetUserName();
      }
      else
      {
         return OfflineLeaderBoard.Instance.GetUserName();
      }
   }

   public void SendLeaderboard(int score)
   {
      if (OnlineEnabled)
      {
         PlayFabManager.Instance.SendLeaderboard(score);
      }
      else
      {
         OfflineLeaderBoard.Instance.SendLeaderboard(score);
      }
   }

   public void GetPlayerInfo()
   {
      if (OnlineEnabled)
      {
         PlayFabManager.Instance.GetPlayerInfo();
      }
      else
      {
         OfflineLeaderBoard.Instance.GetPlayerInfo();
      }
   }

   public void GetLeaderboard()
   {
      if (OnlineEnabled)
      {
         PlayFabManager.Instance.GetLeaderboard();
      }
      else
      {
         OfflineLeaderBoard.Instance.GetLeaderboard();
      }
   }

   public void GetHighestScore()
   {
      if (OnlineEnabled)
      {
         PlayFabManager.Instance.GetHighestScore();
      }
      else
      {
         OfflineLeaderBoard.Instance.GetHighestScore();
      }
   }


   public void InvokePlayerLoggedIn(string userName)
   {
      PlayerLoggedIn?.Invoke(userName);
   }

   public void InvokeLeaderBoardAroundPlayGot(GetLeaderboardAroundPlayerResult result)
   {
      OnLeaderBoardAroundPlayerRetrieved?.Invoke(result);
   }

   public void InvokeLeaderBoardGot(List<PlayerLeaderboardEntry> result)
   {
      OnLeaderBoardRetrieved?.Invoke(result);
   }

   public void InvokeHighestScoreGot(GetLeaderboardResult result)
   {
      OnHighestScoreRetrieved?.Invoke(result);
   }
}
