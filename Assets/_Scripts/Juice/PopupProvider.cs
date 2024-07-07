using System.Collections.Generic;
using UnityEngine;

public class PopupProvider : MonoBehaviour
{
   [SerializeField] private PopupLogic _popupLogic;

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

   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.Keypad2))
      {
         ShowPopup("RoomClear");
      }
   }

   private void SetupDict()
   {
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

   public void ShowPopup(string id)
   {
      if (_runningPopupAlready)
      {
         return;
      }
      
      _popupLogic.gameObject.SetActive(true);
      _popupLogic.SetUp(_popupDataDict[id]);
      _popupLogic.Activate();
      _runningPopupAlready = true;
   }

}
