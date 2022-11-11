using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using Photon.Pun;

public class Character : MonoBehaviour , IPunObservable
{
    [SerializeField] private SpriteRenderer MainSprite;
    [SerializeField] private Rigidbody2D Rigidbody;
    [SerializeField] private TMP_Text Nickname;
    [SerializeField] private TMP_Text Stats;
    [SerializeField] private ParticleSystem HitParticle;
    [Header("Splash area")]
    [SerializeField] private float Offset;
    [SerializeField] private Vector2 Size;
    [Header("Sprite directions")]
    [SerializeField] private Sprite Front;
    [SerializeField] private Sprite Back;
    [SerializeField] private Sprite Left;
    [SerializeField] private Sprite Right;

    [HideInInspector] public NetworkManager NetworkManager;
    [HideInInspector] public bool CanEnterRoom;

    [Header("Network Fields")]
    //network
    [HideInInspector] public PhotonView PhotonView; 
    [HideInInspector] public InputManager InputManager; 

    private int _CurrentSide;
    [SerializeField] private int CurrentSide
    {
        get { return _CurrentSide; }
        set { _CurrentSide = value;
            switch(_CurrentSide)
            {
                case 0:
                    MainSprite.sprite = Right;
                    break;
                case 1:
                    MainSprite.sprite = Left;
                    break;
                case 2:
                    MainSprite.sprite = Back;
                    break;
                case 3:
                    MainSprite.sprite = Front;
                    break;

            }
        }
    }
    private int _Gems;
    [SerializeField]
    private int Gems
    {
        get { return _Gems; }
        set
        {
            _Gems = value;
            UpdateStats();
        }
    }

    private float _Health;
    [SerializeField]
    private float Health
    {
        get { return _Health; }
        set
        {
            _Health = value;
            if (Health <= 0) PhotonNetwork.LeaveRoom();
            UpdateStats();
        }
    }
    private void UpdateStats()
    {
        Stats.text = "stats:\n" +
            "<color=red>HP " + Health + "</color>\n" +
            "<color=green>GEMS " + Gems;
    }
    [PunRPC]
    public void HitMP(float X,float Y)
    {
        Vector2 MousePosition = new Vector2(X,Y);
        Vector2 Pos = MousePosition - new Vector2(transform.position.x, transform.position.y);
        float Angle = Mathf.Atan2(Pos.y, Pos.x) * Mathf.Rad2Deg - 90f;
        Hit(Angle);
        if(PhotonView.IsMine)
        {
            Debug.DrawLine(transform.position, (Pos.normalized * Offset + new Vector2(transform.position.x, transform.position.y)),Color.green,2f);
            Collider2D[] Cols = Physics2D.OverlapBoxAll(Pos.normalized*Offset + new Vector2(transform.position.x, transform.position.y), Size, Angle);
            foreach(Collider2D i in Cols)
            {
                if(i.gameObject != gameObject && i.GetComponent<Character>())
                {
                    Character DamagedPlayer = i.GetComponent<Character>();
                    DamagedPlayer.PhotonView.RPC(nameof(DamagedPlayer.AddDamage), DamagedPlayer.PhotonView.Owner, 10f);
                    break;
                }
            }
        }
    }
    public void Hit(float Angle)
    {
        HitParticle.transform.rotation = Quaternion.Euler(0,0,Angle);
        HitParticle.Play();
    }

    [PunRPC]
    public void AddDamage(float Damage)
    {
        Health -= Damage;
    }
    public void AddMove(Vector2 Direction)
    {
        if (Direction.x > 0) {CurrentSide = 0; }
        if (Direction.x < 0) {CurrentSide = 1; }
        if (Direction.y > 0) {CurrentSide = 2; }
        if (Direction.y < 0) {CurrentSide = 3; }

        Rigidbody.MovePosition(new Vector2(transform.position.x,transform.position.y) + Direction * 0.1f);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(CurrentSide);
            stream.SendNext(Health);
            stream.SendNext(Gems);
        }
        else
        {
            CurrentSide = (int) stream.ReceiveNext();
            Health = (float) stream.ReceiveNext();
            Gems = (int) stream.ReceiveNext();
        }
                
    }
    private void Awake()
    {
        NetworkManager = NetworkManager.Instance;
    }
    private void Start()
    {
        NetworkManager.Players.Add(this);
        PhotonView = GetComponent<PhotonView>();
        Nickname.text = PhotonView.Owner.NickName;
        Health = 100f;
        Gems = 1;
    }
    private float CanEnterRoomt = 0;
    private void Update()
    {
        if (!CanEnterRoom) CanEnterRoomt += Time.deltaTime;
        if (CanEnterRoomt >= 0.5f)
        {
            CanEnterRoomt = 0;
            CanEnterRoom = true;
        }
    }
    private void OnDestroy()
    {
        NetworkManager.Players.Remove(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(transform.position.x, transform.position.y + Offset), Size);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(PhotonView.IsMine)
        {
            if(collision.gameObject.GetComponent<Drop>())
            {
                Drop drop = collision.gameObject.GetComponent<Drop>();
                drop.gameObject.GetComponent<PhotonView>().RPC(nameof(drop.Take),RpcTarget.All);
                Gems += 1;
            }
        }
    }
}
