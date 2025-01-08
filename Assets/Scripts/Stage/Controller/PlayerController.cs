using FirstVillain.EventBus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private MinigameController _minigameController;
    [SerializeField] private Camera _playerCam;
    private bool _isPushed = false;

    private PlayerMove _playerMove;

    private E_TEAM _currentTeam = E_TEAM.None;
    private PlayerInfo _tableData;
    public bool IsBlock { get; private set; }

    private void Start()
    {
        _playerMove = GetComponent<PlayerMove>();
        EventBus.Instance.Subscribe<EventSendMinigamePoint>(OnGetMinigamePoint);
    }

    private void OnDestroy()
    {
        EventBus.Instance.Unsubscribe<EventSendMinigamePoint>(OnGetMinigamePoint);
    }

    public void SetData(PlayerInfo info)
    {
        var camData = _playerCam.GetUniversalAdditionalCameraData();
        camData.cameraStack.Add(UIManager.Instance.GetUICam());

        _tableData = info;
        //_currentTeam = info.Team;
        _currentTeam = E_TEAM.Red;
    }

    private void Update()
    {
        if(IsBlock)
        {
            //떨어졌을 때, 시간 종료 시
            return;
        }

        _playerMove.Rotate(Input.GetAxis("Mouse X"));
                
        var _moveHorizontal = Input.GetAxis("Horizontal");
        var _moveVertical = Input.GetAxis("Vertical");

        _playerMove.Move(_moveHorizontal, _moveVertical, _tableData.SPEED);
              
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _playerMove.Jump();
        }

        if (!_isPushed && Input.GetKeyDown(KeyCode.Q))
        {
            PushStart();
            //미니게임 스타트
        }

        if (_isPushed && Input.GetKeyUp(KeyCode.Q))
        {
            //미니게임 스탑
            EventBus.Instance.Publish(new EventMinigameStop());
        }
    }

    private void PushStart()
    {
        _isPushed = true;
        _minigameController.gameObject.SetActive(true);
        _minigameController.StartGame();
    }
    private void Push(float point)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _tableData.RANGE);

        foreach (var collider in colliders)
        {
            if (LayerMask.LayerToName(collider.gameObject.layer) == "Objects")
            {
                var prop = collider.GetComponent<PropController>();
                if(prop != null)
                {
                    prop.Explode(_tableData.STR, transform.position, point, _currentTeam);
                }
            }

        }
        _isPushed = false;
    }
    
    private void OnGetMinigamePoint(EventSendMinigamePoint e)
    {
        _minigameController.gameObject.SetActive(false);
        Push(e.MinigamePoint);
    }

    public void Respawn()
    {
        IsBlock = false;
    }

    public void Block()
    {
        IsBlock = true;
        EventBus.Instance.Publish(new EventMinigameStop());
    }
    public void GameOver()
    {
        var camData = _playerCam.GetUniversalAdditionalCameraData();
        camData.cameraStack.Clear();
    }

    #region Collision

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Remove"))
        {
            Block();
            StageManager.Instance.PlayerFall(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            var item = other.GetComponent<StageItem>();
            item.UseItem();
            EventBus.Instance.Publish(new EventItemRemoved(item));
        }
    }
    #endregion Collision
}
