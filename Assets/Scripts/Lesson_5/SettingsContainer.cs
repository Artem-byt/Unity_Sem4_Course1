using UnityEngine;

public class SettingsContainer : Singleton<SettingsContainer>
{
    public SpaceShipSettings SpaceShipSettings => _spaceShipSettings;

    [SerializeField] private SpaceShipSettings _spaceShipSettings;
}
