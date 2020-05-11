using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class ProgramBlockConfiguratorSlot : MonoBehaviour
{
    public ProgramBlockConfigurator PBConfigurator;

    public AudioClip AssignAC;
    public AudioClip RemoveAC;

    private AProgramBlock _assignedPB = null;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void AssignBlock(AProgramBlock programBlock)
    {
        if (programBlock == null)
        {
            _assignedPB = null;
            _audioSource.PlayOneShot(RemoveAC);
            return;
        }

        _audioSource.PlayOneShot(AssignAC);


        programBlock.transform.SetParent(transform);

        programBlock.transform.DOMove(transform.position, 0.2f);
        programBlock.transform.rotation = transform.rotation;
        programBlock.SetState(AProgramBlock.BlockState.IN_SLOT, false);
        _assignedPB = programBlock;
        _assignedPB.Grabbable.OnGrabBegin += RemoveBlock;
        PBConfigurator.Show(_assignedPB);
    }

    public void RemoveBlock()
    {
        if(_assignedPB != null)
        {
            _assignedPB.Grabbable.OnGrabBegin -= RemoveBlock;
            _assignedPB = null;
            PBConfigurator.Hide();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("ProgramBlock"))
        {
            if (other.GetComponent<Rigidbody>().useGravity)
            {
                AProgramBlock pb = other.GetComponent<AProgramBlock>();
                if(_assignedPB != null)
                {
                    Vector3 force = OVRManager.instance.transform.position - pb.transform.position;
                    force = force.normalized * 1f;
                    force.y = 2f;
                    pb.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    pb.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
                }
                else
                {
                    AssignBlock(other.GetComponent<AProgramBlock>());
                }
            }
        }
    }
}
