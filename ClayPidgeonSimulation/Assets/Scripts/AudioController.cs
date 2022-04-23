using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

    public static AudioClip CollisionSFX, ShotFiredSFX;
    public static AudioSource audioSrc;

    // Loading in the Audio sources
    private void Start() {
        CollisionSFX = Resources.Load<AudioClip>("CollisionSFX");
        ShotFiredSFX = Resources.Load<AudioClip>("ShotFiredSFX");

        audioSrc = GetComponent<AudioSource>();
    }

    // Sound effects being triggered by collision detection
    private void Update() {
       
            if (OctreeNode.collision) {
                PlaySound("CollisionSfx");
                OctreeNode.collision = false;
            }
            // if (InterceptController.shotFired) {
            //     PlaySound("ShotFiredSfx");
            //     InterceptController.shotFired = false;
            // }
    }

    public static void PlaySound(string clip) {
        switch (clip) {
            case "CollisionSfx" :
                audioSrc.PlayOneShot(CollisionSFX);
                break;
            case "ShotFiredSfx" :
                audioSrc.PlayOneShot(ShotFiredSFX);
                break;
        }
    }
}
