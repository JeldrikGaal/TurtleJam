using System.Collections.Generic;
using UnityEngine;

public class PopupProvider : MonoBehaviour
{
   [SerializeField] private PopupLogic _popupLogic;

   [SerializeField] private PopupLogic.PopupData _defaultData;
   [SerializeField] private List<PopupLogic.PopupData> _dataList;
   
   private Dictionary<string, PopupLogic.PopupData> _popupDataDict = new Dictionary<string, PopupLogic.PopupData>();
   
   private bool _runningPopupAlready;

   public static PopupProvider Instance;
   
   private void Awake()
   {
      if (Instance == null)
      {
         Instance = this;
      }
      else
      {
         Destroy(this);
      }
      
      PopupLogic.PopupEnded += ReactToPopupAnimEnd;
      SetupDict();
   }

   private void OnDestroy()
   {
      PopupLogic.PopupEnded -= ReactToPopupAnimEnd;
   }

   private void SetupDict()
   {
      _dataList.Add(_defaultData);
      foreach (var entry in _dataList)
      {
         _popupDataDict.Add(entry.ID, entry);
      }
   }

   private void ReactToPopupAnimEnd(PopupLogic.PopupData data)
   {
      if (_runningPopupAlready)
      {
         _runningPopupAlready = false;
      }
      else
      {
         Debug.LogError("Ending popup while none is registered as running shouldn't happen! ");
      }
   }

   public void ShowPopupFromText(string text)
   {
      _popupLogic.gameObject.SetActive(true);
      _popupLogic.SetUp(_popupDataDict["Default"]);
      _popupLogic.Activate("", text);
      _runningPopupAlready = true;
   }

   public void ShowPopup(string id, string bonusText = "")
   {
      if (_runningPopupAlready)
      {
         return;
      }
      
      _popupLogic.gameObject.SetActive(true);
      _popupLogic.SetUp(_popupDataDict[id]);
      _popupLogic.Activate(bonusText);
      _runningPopupAlready = true;
   }

}
