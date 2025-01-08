using FirstVillain.Entities;
using FirstVillain.EventBus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropController : MonoBehaviour
{
    [SerializeField] private GameObject _spawnFx;
    [SerializeField] private GameObject _trailFx;
    //생성 시 FX
    private E_TEAM _hitTeam = E_TEAM.None;

    private Rigidbody _rigidBody;

    private JPropInfoData _data;

    private void OnEnable()
    {
        _hitTeam = E_TEAM.None;
    }
    public void SetData(JPropInfoData data, Bounds bound)
    {
        InitData(data);

        var randomPos = new Vector3(
            Random.Range(bound.min.x, bound.max.x),
            Random.Range(bound.min.y, bound.max.y),
            Random.Range(bound.min.z, bound.max.z));

        transform.position = randomPos;
    }

    public void SetData(JPropInfoData data)
    {
        transform.localPosition = Vector3.zero;
        InitData(data);
    }

    private void InitData(JPropInfoData data)
    {
        _trailFx.SetActive(false);
        //TODO : 생성 시 FX 추가해야함
        //_spawnFx.SetActive(true);

        _rigidBody = GetComponent<Rigidbody>();

        _data = data;

        _rigidBody.mass = _data.Mass;
    }

    public void Explode(int str, Vector3 position, float dist, E_TEAM team)
    {
        Vector3 dir = transform.position - position;
        int rndAngle = Random.Range(-45, -30);
        dir = Quaternion.AngleAxis(rndAngle, Vector3.right) * dir;

        _rigidBody.AddForce(dir.normalized * str * dist, ForceMode.Impulse);

        _trailFx.SetActive(true);
        _hitTeam = team;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Remove"))
        {
            if(_hitTeam != E_TEAM.None)
            {
                _trailFx.SetActive(false);
                //_spawnFx.SetActive(false);
                StageManager.Instance.UpdateScore(_hitTeam, _data);
                EventBus.Instance.Publish(new EventPropRemoved(this));
            }
        }
    }
}
