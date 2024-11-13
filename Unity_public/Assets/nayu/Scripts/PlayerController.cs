using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed, gravityModifier, jumpPower; //���x�Əd�͂̑傫��,�W�����v��
    public CharacterController charaCon;
    private Vector3 moveInput;

    public Transform camTrans;
    public float mouseSensitivity;
    public bool invertX;
    public bool invertY;

    private bool canJump;
    public Transform groundCheckPoint;
    public LayerMask whatIsGround;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //moveInput.x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        //moveInput.z = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;


        //store y velocity 
        float ystore = moveInput.y;

        Vector3 verMove = transform.forward * Input.GetAxis("Vertical");
        Vector3 horiMove = transform.right * Input.GetAxis("Horizontal");

        moveInput = horiMove + verMove;
        moveInput.Normalize();
        moveInput = moveInput * moveSpeed;

        moveInput.y = ystore;
        moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;

        if (charaCon.isGrounded)
        {
            moveInput.y = Physics.gravity.y * gravityModifier * Time.deltaTime;
        }

        //�W�����v
        canJump = Physics.OverlapSphere(groundCheckPoint.position, .25f, whatIsGround).Length > 0; //�n�ʂɂ��Ă���0.25�o������
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            moveInput.y = jumpPower;
        }
        charaCon.Move(moveInput * Time.deltaTime);

        charaCon.Move(moveInput * Time.deltaTime);

        //�J�����̉�]����
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;
        if (invertX)//�}�E�X���]�������Ȃ�B
        {
            mouseInput.x = -mouseInput.x;
        }
        if (invertY)
        {
            mouseInput.y = -mouseInput.y;
        }
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);
        camTrans.rotation = Quaternion.Euler(camTrans.rotation.eulerAngles + new Vector3(-mouseInput.y, 0f, 0f));
    }
}