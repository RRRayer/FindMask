using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoBall : MonoBehaviour
{
    [Header("이벤트 채널 설정")]
    [Tooltip("디스코볼 효과를 시작시키는 이벤트 채널입니다.")]
    public VoidEventChannelSO startDiscoEvent;
    [Tooltip("디스코볼 효과를 중지시키는 이벤트 채널입니다.")]
    public VoidEventChannelSO stopDiscoEvent;

    [Header("라이트 설정")]
    public Light[] allSpotlights;
    public int numberOfGroups = 3;

    [Header("그룹 전환 설정")]
    public float groupActiveDuration = 5f;

    [Header("중앙 PointLight 설정")]
    public Light centralPointLight;

    [Header("색상 설정")]
    public Color[] discoColors;

    [Header("점멸 효과 설정")]
    public float minBlinkInterval = 0.5f;
    public float maxBlinkInterval = 1.0f;
    public float minOnTime = 0.3f;
    public float maxOnTime = 0.7f;
    public int flashCountPerColor = 1;

    [Header("실시간 회전 설정")]
    public float lightRotationSpeed = 0.5f;
    public Vector2 minMaxXRotation = new Vector2(45f, 90f);
    public bool randomizeYRotation = true;

    [Header("회전 속도")]
    public float rotationSpeed = 30f;

    private List<List<Light>> _logicalLightGroups = new List<List<Light>>();
    private List<Coroutine> _runningCoroutines = new List<Coroutine>(); // 실행중인 코루틴 관리 리스트
    private bool _isDiscoActive = false;

    private void Awake()
    {
        // 조명을 논리적 그룹으로 나누는 로직
        if (allSpotlights != null && allSpotlights.Length > 0)
        {
            if (numberOfGroups <= 0) numberOfGroups = 1;
            for (int i = 0; i < numberOfGroups; i++)
            {
                _logicalLightGroups.Add(new List<Light>());
            }
            for (int i = 0; i < allSpotlights.Length; i++)
            {
                _logicalLightGroups[i % numberOfGroups].Add(allSpotlights[i]);
            }
        }
    }

    private void OnEnable()
    {
        // 이벤트 채널에 메소드 등록
        if (startDiscoEvent != null) startDiscoEvent.OnEventRaised += StartDisco;
        if (stopDiscoEvent != null) stopDiscoEvent.OnEventRaised += StopDisco;
    }

    private void OnDisable()
    {
        // 이벤트 채널에서 메소드 등록 해제 (메모리 누수 방지)
        if (startDiscoEvent != null) startDiscoEvent.OnEventRaised -= StartDisco;
        if (stopDiscoEvent != null) stopDiscoEvent.OnEventRaised -= StopDisco;
        
        // 비활성화 시 확실하게 모든 효과 중지
        StopDisco();
    }

    void Update()
    {
        // 디스코볼 자체의 회전은 활성화 여부와 상관없이 계속 될 수 있습니다.
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 디스코 효과를 시작하는 메소드 (이벤트에 의해 호출됨)
    /// </summary>
    public void StartDisco()
    {
        if (_isDiscoActive) return;
        _isDiscoActive = true;

        // 이전에 실행중인 모든 코루틴을 정리
        StopAllCoroutines();
        _runningCoroutines.Clear();

        // 마스터 코루틴 하나만 시작하고 관리
        var masterCoroutine = StartCoroutine(DiscoLoop());
        _runningCoroutines.Add(masterCoroutine);
    }

    /// <summary>
    /// 디스코 효과를 중지하는 메소드 (이벤트에 의해 호출됨)
    /// </summary>
    public void StopDisco()
    {
        if (!_isDiscoActive) return;
        _isDiscoActive = false;

        // 모든 코루틴 중지 (가장 확실한 방법)
        StopAllCoroutines();
        _runningCoroutines.Clear();

        // 모든 조명 끄기
        foreach (var group in _logicalLightGroups) TurnOffLightsInGroup(group);
        if (centralPointLight != null) centralPointLight.enabled = false;
    }

    IEnumerator DiscoLoop()
    {
        // --- 초기화 ---
        int currentGroupIndex = 0;
        float groupSwitchTimer = 0f;

        var fromRotations = new Dictionary<Light, Quaternion>();
        var toRotations = new Dictionary<Light, Quaternion>();
        var lerpProgress = new Dictionary<Light, float>();

        // 첫 번째 그룹 준비
        List<Light> currentGroup = _logicalLightGroups.Count > 0 ? _logicalLightGroups[0] : new List<Light>();
        InitializeRotationsForGroup(currentGroup, fromRotations, toRotations, lerpProgress);

        // --- 메인 루프 ---
        while (_isDiscoActive)
        {
            float deltaTime = Time.deltaTime;
            groupSwitchTimer += deltaTime;
            
            // 그룹 전환 로직
            if (groupSwitchTimer >= groupActiveDuration && _logicalLightGroups.Count > 0)
            {
                groupSwitchTimer = 0;
                TurnOffLightsInGroup(currentGroup); // 이전 그룹 끄기

                currentGroupIndex = (currentGroupIndex + 1) % _logicalLightGroups.Count;
                currentGroup = _logicalLightGroups[currentGroupIndex];
                
                InitializeRotationsForGroup(currentGroup, fromRotations, toRotations, lerpProgress);
            }

            // 활성 그룹에 대한 점멸 및 회전 처리
            // (내부에서 자체적으로 타이머를 돌며 점멸)
            yield return StartCoroutine(ProcessActiveGroupAndCentralLight(currentGroup, fromRotations, toRotations, lerpProgress));
        }
    }

    private void InitializeRotationsForGroup(List<Light> group, Dictionary<Light, Quaternion> from, Dictionary<Light, Quaternion> to, Dictionary<Light, float> progress)
    {
        from.Clear();
        to.Clear();
        progress.Clear();
        foreach(var light in group)
        {
            if(light == null) continue;
            from[light] = light.transform.localRotation;
            to[light] = GenerateRandomRotation();
            progress[light] = 0f;
        }
    }

    IEnumerator ProcessActiveGroupAndCentralLight(List<Light> groupLights, Dictionary<Light, Quaternion> fromRotations, Dictionary<Light, Quaternion> toRotations, Dictionary<Light, float> lerpProgress)
    {
        // 스포트라이트 켜기
        SetRandomColorsForGroup(groupLights);
        TurnOnLightsInGroup(groupLights, true);

        // 중앙 라이트 켜기
        if (centralPointLight != null)
        {
            centralPointLight.enabled = true;
            if (discoColors.Length > 0)
                centralPointLight.color = discoColors[Random.Range(0, discoColors.Length)];
        }
        
        float onTimer = 0f;
        float onDuration = Random.Range(minOnTime, maxOnTime);
        while(onTimer < onDuration)
        {
            if (!_isDiscoActive) yield break; // 중간에 중지 신호가 오면 즉시 종료
            UpdateLightRotations(groupLights, fromRotations, toRotations, lerpProgress);
            onTimer += Time.deltaTime;
            yield return null;
        }

        // 모든 라이트 끄기
        TurnOnLightsInGroup(groupLights, false);
        if (centralPointLight != null) centralPointLight.enabled = false;
        
        float offTimer = 0f;
        float offDuration = Random.Range(minBlinkInterval, maxBlinkInterval);
        while (offTimer < offDuration)
        {
            if (!_isDiscoActive) yield break; // 중간에 중지 신호가 오면 즉시 종료
            UpdateLightRotations(groupLights, fromRotations, toRotations, lerpProgress);
            offTimer += Time.deltaTime;
            yield return null;
        }
    }
    
    private void UpdateLightRotations(List<Light> lights, Dictionary<Light, Quaternion> from, Dictionary<Light, Quaternion> to, Dictionary<Light, float> progress)
    {
        foreach (var light in lights)
        {
            if (light == null || light.type != LightType.Spot) continue;

            progress[light] += Time.deltaTime * lightRotationSpeed;
            light.transform.localRotation = Quaternion.Slerp(from[light], to[light], progress[light]);

            if (progress[light] >= 1f)
            {
                from[light] = to[light];
                to[light] = GenerateRandomRotation();
                progress[light] = 0f;
            }
        }
    }

    private void SetRandomColorsForGroup(List<Light> groupLights)
    {
        bool hasColors = discoColors != null && discoColors.Length > 0;
        if (!hasColors) return;

        foreach (var light in groupLights)
        {
            if (light != null)
            {
                light.color = discoColors[Random.Range(0, discoColors.Length)];
            }
        }
    }

    private Quaternion GenerateRandomRotation()
    {
        float newX = Random.Range(minMaxXRotation.x, minMaxXRotation.y);
        float newY = randomizeYRotation ? Random.Range(0f, 360f) : 0;
        return Quaternion.Euler(newX, newY, 0);
    }

    private void TurnOffLightsInGroup(List<Light> groupLights)
    {
        TurnOnLightsInGroup(groupLights, false);
    }

    private void TurnOnLightsInGroup(List<Light> groupLights, bool state)
    {
        if (groupLights == null) return;
        foreach (Light light in groupLights)
        {
            if (light != null) light.enabled = state;
        }
    }

    IEnumerator CentralPointLightBlinkProcess()
    {
        bool hasColors = discoColors != null && discoColors.Length > 0;
        
        while (true)
        {
            // 점멸할 때마다 새로운 랜덤 색상 선택
            if (hasColors)
            {
                centralPointLight.color = discoColors[Random.Range(0, discoColors.Length)];
            }
            
            // 라이트 켜기
            centralPointLight.enabled = true;
            yield return new WaitForSeconds(Random.Range(minOnTime, maxOnTime));

            // 라이트 끄기
            centralPointLight.enabled = false;
            yield return new WaitForSeconds(Random.Range(minBlinkInterval, maxBlinkInterval));
        }
    }
}