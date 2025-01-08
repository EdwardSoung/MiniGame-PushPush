using FirstVillain.EventBus;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelStage : UIBase
{
    [SerializeField] private Button _settingButton;
    [SerializeField] private TextMeshProUGUI _spawnTimer;
    [SerializeField] private TextMeshProUGUI _playTimer;

    [SerializeField] private TextMeshProUGUI _redTeamScoreText;
    [SerializeField] private TextMeshProUGUI _blueTeamScoreText;
    public override void Open()
    {
        _spawnTimer.text = string.Empty;
        UpdatePlayTimer((int)Constants.PLAY_TIME);
        base.Open();
    }

    public override void CloseAction()
    {
        base.CloseAction();
    }

    public void UpdatePlayTimer(int time)
    {
        _playTimer.text = $"{time / 60} : {string.Format("{0:00}", time % 60)}";
    }

    public void UpdateSpawnTimer(int time)
    {
        string timeValue = time.ToString();
        if(time == 0)
        {
            timeValue = "START";
            StartCoroutine(DelayCloseStart());
        }
        _spawnTimer.text = timeValue;
    }

    private IEnumerator DelayCloseStart()
    {
        yield return new WaitForSeconds(.5f);
        _spawnTimer.text = string.Empty;
    }

    public void UpdateScore(E_TEAM team, int score)
    {
        var textObj = team == E_TEAM.Red ? _redTeamScoreText : _blueTeamScoreText;

        textObj.SetText(score.ToString());
    }
}
