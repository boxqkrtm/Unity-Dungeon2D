using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchPad : MonoBehaviour
{
    public Vector2 intDiff;
    private RectTransform _touchPad;
    //터치 입력중에 방향컨트롤러와
    private int _touchId = -1;
    //입력이 시작되는 좌표
    private Vector3 _startPos = Vector3.zero;
    //방향컨트롤러가 원으로 움직이는 반지름
    public float _dragRadius = 60f;
    //플레잉의 움직임으 관리하는 Playermovement 스크립트와 연결
    //방향키가 변경되면 캐릭터에 신호를 보낸다.
    public bool _buttonPressed = false;

    private void Start()
    {
        //터치패드의 RectTransform 오브젝트를 가져옵니다.   
        _touchPad = GetComponent<RectTransform>();
        //터치 패드의 좌표를 가져옵니다. 움직임의 기준값이 됩니다.
        _startPos = _touchPad.position;
    }

    private void Update()
    {
        //버튼이 눌렸는지 확인해놓기
        //_buttonPressed = true;
    }

    public void ButtonUp()
    {
        _buttonPressed = false;
        HandleInput(_startPos);
    }

    public void ButtonDown()
    {
        _buttonPressed = true;
        HandleInput(_startPos);
    }

    private void FixedUpdate()
    {
        //모바일에서는 터치패드 방식으로 여러 터치 입력을 받아 처리가능
        HandleTouchInput();
        //모바일이 아닌 PC나 유니티 에디터 상에서 작동할 때는 터치 입력이
        //아닌 마우스로 입력받습니다.
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_StANDALONE_WIN || UNITY_WEBPLAYER
        HandleInput(Input.mousePosition);
#endif
    }

    void HandleTouchInput()
    {
        //터치 아이디를 매기기 위한 번호
        int i = 0;

        //터치 입력은 한 번에 여러개가 들어올 수 있음 하나 이상 입력되면 실행
        if (Input.touchCount > 0)
        {
            foreach (var touch in Input.touches)
            {
                //터치 아이디를 먀기기 위한 번호 1 증가
                i++;
                //현재 터치 입력의 x, y 좌표구함
                Vector3 touchPos = new Vector3(touch.position.x, touch.position.y);
                //터치 입력이 방금 시작되었다면, TouchPhase.Begin이면
                if (touch.phase == TouchPhase.Began)
                {
                    //그리고 터치의 좌표가 현재 방향키 범위 내에 있다면
                    if (touch.position.x <= (_startPos.x + _dragRadius))
                    {
                        //이 터치 아이디를 기준으로 방향 컨트롤러 조작
                        _touchId = i;
                    }
                }
                //터치 입력이 움직였다거나, 가만히 있는 상황이라면
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    //터치 아이디로 지정된 경우에만
                    if (_touchId == i)
                    {
                        //좌표를 입력을 받아들입니다.
                        HandleInput(touch.position);
                    }
                }
                //터치가 입력이 끝났다면
                if (touch.phase == TouchPhase.Ended)
                {
                    //입력받고자 했던 터치 아이디라면
                    if (_touchId == i)
                    {
                        //터치 아이디를 해제한다.
                        _touchId = -1;
                    }
                }
            }
        }
    }

    void HandleInput(Vector3 input)
    {
        //버튼이 눌러진 상황에 좌표의 차이 구함
        if (_buttonPressed)
        {
            Vector3 diffVector = (input - _startPos);
            if (diffVector.sqrMagnitude > _dragRadius * _dragRadius)
            {
                diffVector.Normalize();
                _touchPad.position = _startPos + diffVector * _dragRadius;
            }
            else
            {
                //현재 입력좌표에 방향키를 이동시킵니다.
                _touchPad.position = input;
            }

        }
        else
        {
            _touchPad.position = _startPos;
        }
        Vector3 diff = _touchPad.position - _startPos;
        //방향키의 방향을 유지한 채로, 거리를 나누어 방향만 구합니다.
        Vector2 normDiff = new Vector3(diff.x / _dragRadius, diff.y / _dragRadius);
        intDiff = new Vector2(Mathf.Round(normDiff.x), Mathf.Round(normDiff.y));
    }
}
