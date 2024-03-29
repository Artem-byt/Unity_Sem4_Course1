using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ShipController : NetworkMovableObject
{
    public string PlayerName { get { return _playerName; } set { _playerName = value; gameObject.name = value;} }
    protected override float _speed => _shipSpeed; 
    [SerializeField] private Transform _cameraAttach; 
    private CameraOrbit _cameraOrbit; 
    private PlayerLabel _playerLabel; 
    private float _shipSpeed; 
    private Rigidbody _rb;
    [SerializeField] private EndGamer _endGamer;
    [SerializeField][SyncVar] private string _playerName;
    [SerializeField] private GameObject _spawns;
    [SerializeField][SyncVar] private int _numberOfPoints;

    public int NumberOfPoints { get { return _numberOfPoints; } }

    private void OnGUI() 
    { 
        if (_cameraOrbit == null) 
        { 
            return; 
        } 
        _cameraOrbit.ShowPlayerLabels(_playerLabel); 
    }

    public override void OnStartAuthority()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            return;
        }
        gameObject.name = _playerName;
        _cameraOrbit = FindObjectOfType<CameraOrbit>();
        _cameraOrbit.Initiate(_cameraAttach == null ? transform : _cameraAttach);
        _playerLabel = GetComponentInChildren<PlayerLabel>();
        base.OnStartAuthority();
    }

    protected override void HasAuthorityMovement()
    {
        var spaceShipSettings = SettingsContainer.Instance?.SpaceShipSettings;
        if (spaceShipSettings == null)
        {
            return;
        }
        var isFaster = Input.GetKey(KeyCode.LeftShift);
        var speed = spaceShipSettings.ShipSpeed;
        var faster = isFaster ? spaceShipSettings.Faster : 1.0f;
        _shipSpeed = Mathf.Lerp(_shipSpeed, speed * faster, SettingsContainer.Instance.SpaceShipSettings.Acceleration);
        var currentFov = isFaster ? SettingsContainer.Instance.SpaceShipSettings.FasterFov : SettingsContainer.Instance.SpaceShipSettings.NormalFov;
        _cameraOrbit.SetFov(currentFov, SettingsContainer.Instance.SpaceShipSettings.ChangeFovSpeed);
        var velocity = _cameraOrbit.transform.TransformDirection(Vector3.forward) * _shipSpeed;
        _rb.velocity = velocity * Time.deltaTime;

        if (!Input.GetKey(KeyCode.C))
        {
            var targetRotation = Quaternion.LookRotation(Quaternion.AngleAxis(_cameraOrbit.LookAngle, -transform.right) * velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
        }
    }

    protected override void FromServerUpdate() { }
    protected override void SendToServer() { }

    [ClientCallback] 
    private void LateUpdate() 
    { 
        _cameraOrbit?.CameraMovement();
        gameObject.name = _playerName;
    }

    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        RpcRespawn();
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
        _numberOfPoints++;
        var objects = FindObjectOfType<SolarSystemNetworkManager>().Players;
        CheckEndGame(objects);

    }

    [ServerCallback]
    private void CheckEndGame(Dictionary<int, ShipController> players)
    {
        int sum = 0;
        foreach(var player in players)
        {
            sum += player.Value.NumberOfPoints;
        }

        if (sum == 6) 
        {
            var playersName = new List<string>();
            var playersPoints = new List<int>();
            
            foreach (var player in players)
            {
                playersName.Add(player.Value.PlayerName);
                playersPoints.Add(player.Value.NumberOfPoints);
            }
            foreach(var player in players)
            {
                player.Value.RpcEndGame(playersName.ToArray(), playersPoints.ToArray());
            }
            
            
        }
    }

    [ClientRpc]
    public void RpcEndGame(string[] playersName, int[] playersPoints)
    {
        gameObject.SetActive(false);
        if (hasAuthority)
        {
            _endGamer.EndGame(playersName, playersPoints);
        }


    }

    [ClientRpc]
    private async void RpcRespawn()
    {
        gameObject.SetActive(false);

        if (hasAuthority)
        {
            
            var spawns = _spawns.GetComponentsInChildren<NetworkStartPosition>();
            var number = Random.Range(0, spawns.Length - 1);

            gameObject.transform.position = spawns[number].transform.position;
        }
        await Task.Delay(1000);
        gameObject.SetActive(true);
    }

}
