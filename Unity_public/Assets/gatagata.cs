using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// ランダム値を使用した揺れ
/// </summary>
public class ShakeByRandom : MonoBehaviour
{
    /// <summary>
    /// 揺れ情報
    /// </summary>
    private struct ShakeInfo
    {
        public ShakeInfo(float strength, float vibrato)
        {
            Strength = strength;
            Vibrato = vibrato;
        }
        public float Strength { get; } // 揺れの強さ
        public float Vibrato { get; }  // どのくらい振動するか
    }
    private ShakeInfo _shakeInfo;

    private Vector3 _initPosition; // 初期位置
    private bool _isDoShake;       // 揺れ実行中か？
    private float _totalShakeTime; // 揺れ経過時間

    private void Start()
    {
        // 初期位置を保持
        _initPosition = gameObject.transform.position;

        // 揺れを開始（強さ、振動範囲は自由に調整可能）
        StartShake(1f, 1f);
    }

    private void Update()
    {
        if (!_isDoShake) return;
        
        // 揺れ位置情報更新
        gameObject.transform.position = UpdateShakePosition(
            gameObject.transform.position, 
            _shakeInfo, 
            _totalShakeTime, 
            _initPosition);
        
        // 揺れが継続するため、時間は常に加算
        _totalShakeTime += Time.deltaTime;
    }

    /// <summary>
    /// 更新後の揺れ位置を取得
    /// </summary>
    private Vector3 UpdateShakePosition(Vector3 currentPosition, ShakeInfo shakeInfo, float totalTime, Vector3 initPosition)
    {
        // -strength ~ strength の値で揺れの強さを取得
        var strength = shakeInfo.Strength;
        var randomX = Random.Range(-1.0f * strength, strength);
        var randomY = Random.Range(-1.0f * strength, strength);
        
        // 現在の位置に加える
        var position = currentPosition;
        position.x += randomX;
        position.y += randomY;
        
        // 初期位置-vibrato ~ 初期位置+vibrato の間に収める
        var vibrato = shakeInfo.Vibrato;
        var ratio = Mathf.PingPong(totalTime, 1.0f); // フェードイン・アウト効果
        vibrato *= ratio;
        position.x = Mathf.Clamp(position.x, initPosition.x - vibrato, initPosition.x + vibrato);
        position.y = Mathf.Clamp(position.y, initPosition.y - vibrato, initPosition.y + vibrato);
        return position;
    }

    /// <summary>
    /// 揺れ開始
    /// </summary>
    /// <param name="strength">揺れの強さ</param>
    /// <param name="vibrato">どのくらい振動するか</param>
    public void StartShake(float strength, float vibrato)
    {
        // 揺れ情報を設定して開始
        _shakeInfo = new ShakeInfo(strength, vibrato);
        _isDoShake = true;
        _totalShakeTime = 0.0f;
    }
}
