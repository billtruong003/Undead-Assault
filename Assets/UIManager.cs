// using UnityEngine;
// using UnityEngine.UI;
// using Photon.Pun;
// public class UIManager : MonoBehaviour
// {
//     public InputField roomNameInputField;
//     public Dropdown gameModeDropdown;
//     public Dropdown teamDropdown;
//     public Button createRoomButton;
//     public Button joinRoomButton;
//     public Button selectTeamButton;

//     private RoomManager roomManager;

//     private void Start()
//     {
//         roomManager = FindObjectOfType<RoomManager>();

//         // Gán các sự kiện cho nút
//         createRoomButton.onClick.AddListener(OnCreateRoomButtonClicked);
//         joinRoomButton.onClick.AddListener(OnJoinRoomButtonClicked);
//         selectTeamButton.onClick.AddListener(OnSelectTeamButtonClicked);

//         // Thiết lập các lựa chọn trong Dropdown
//         gameModeDropdown.options.Clear();
//         foreach (string mode in System.Enum.GetNames(typeof(RoomManager.GameMode)))
//         {
//             gameModeDropdown.options.Add(new Dropdown.OptionData(mode));
//         }
//         gameModeDropdown.value = 0; // Chọn mục đầu tiên mặc định

//         // Thiết lập các lựa chọn cho team
//         teamDropdown.options.Clear();
//         teamDropdown.options.Add(new Dropdown.OptionData("Team 1"));
//         teamDropdown.options.Add(new Dropdown.OptionData("Team 2"));
//         teamDropdown.value = 0; // Chọn mục đầu tiên mặc định
//     }

//     private void OnCreateRoomButtonClicked()
//     {
//         string roomName = roomNameInputField.text;
//         RoomManager.GameMode gameMode = (RoomManager.GameMode)gameModeDropdown.value;

//         roomManager.CreateRoom(roomName, gameMode);
//     }

//     private void OnJoinRoomButtonClicked()
//     {
//         string roomName = roomNameInputField.text;
//         PhotonNetwork.JoinRoom(roomName);
//     }

//     private void OnSelectTeamButtonClicked()
//     {
//         int team = teamDropdown.value + 1; // Team 1 là 1, Team 2 là 2
//         int playerId = PhotonNetwork.LocalPlayer.ActorNumber;

//         roomManager.AssignTeam(playerId, team);
//     }
// }
