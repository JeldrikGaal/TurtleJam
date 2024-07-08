using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PopupLogic : MonoBehaviour
{
    [SerializeField] private TMP_Text _textField;
    [SerializeField] private Vector3 _uiPos;
    [SerializeField] private Vector3 _screenMiddlePos;
    
    private PopupData _data;

    private const int Width = 1920;

    private Sequence _animSequence;
    
    public static event Action<PopupData> PopupEnded;

    [Serializable]
    public struct PopupData
    {
        public string Text;
        public string ID;
        public PopupPosition Position;
        public float StayDuration;
        public float AnimInSpeed;
        public float AnimOutSpeed;
        public BonusTextPos BonusTextPos;
        
        public PopupData(string text,string id, PopupPosition position, float stayDuration, float animInSpeed, float animOutSpeed, BonusTextPos bonusTextPos)
        {
            Text = text;
            ID = id;
            Position = position;
            StayDuration = stayDuration;
            AnimInSpeed = animInSpeed;
            AnimOutSpeed = animOutSpeed;
            BonusTextPos = bonusTextPos;
        }
    }
    
    public enum PopupPosition
    {
        UI,
        MiddleScreen
    }
    
    public enum BonusTextPos
    {
        Start,
        End,
        None
    }
    
    public void SetUp(PopupData data)
    {
        _data = data;
    }

    public void Activate(string bonusText)
    {
        SetText(bonusText);
        PlayAnim();
    }

    private void SetText(string bonusText)
    {
        switch (_data.BonusTextPos)
        {
            case BonusTextPos.Start:
                _textField.text = _data.Text + " " + bonusText;
                break;
            case BonusTextPos.End:
                _textField.text = bonusText + " " + _data.Text;
                break;
            case BonusTextPos.None:
                _textField.text = _data.Text;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private Vector3 GetStartPos()
    {
        switch (_data.Position)
        {
            case PopupPosition.UI:
                return _uiPos;
            case PopupPosition.MiddleScreen:
                return _screenMiddlePos;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void PlayAnim()
    {
        transform.localPosition = GetStartPos();
        transform.localPosition -= Vector3.right * Width;
        _animSequence = DOTween.Sequence();
        _animSequence.Append(transform.DOLocalMoveX(GetStartPos().x, _data.AnimInSpeed)).SetEase(Ease.InOutSine);
        //_animSequence.Insert(_data.AnimInSpeed, _textField.transform.DOPunchScale(_textField.transform.localScale * 1.001f, 0.25f));
        _animSequence.AppendInterval(_data.StayDuration);
        _animSequence.Append(transform.DOLocalMoveX(GetStartPos().x + Width, _data.AnimOutSpeed)).SetEase(Ease.InOutSine);
        _animSequence.OnComplete(() =>
        {
            PopupEnded?.Invoke(_data);
            gameObject.SetActive(false);
            
        });
    }
}
