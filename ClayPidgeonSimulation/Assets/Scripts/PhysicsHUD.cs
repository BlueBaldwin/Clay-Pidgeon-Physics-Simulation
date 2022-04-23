using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PhysicsHUD : MonoBehaviour {
    public bool simulationPaused = false;
    public GameObject menu;
    public Text totalCollisions;
    public Text projectileSpeed;
    public Text fastestPossibleCollisionTime;

    
    private void Awake() {
        simulationPaused = true;
        Time.timeScale = 0;
    }

    private void Update() {
        projectileSpeed.text = "PROJECTILE SPEED[mps]: " + InterceptController.projectileSpeed.ToString();
        totalCollisions.text = "TARGETS HIT: " + OctreeNode.collisionCount.ToString();

        if (simulationPaused) {
            menu.SetActive(true);
            Time.timeScale = 0;
            simulationPaused = true;
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Application.Quit();
            }
        }
        else if (!simulationPaused) {
            menu.SetActive(false);
            Time.timeScale = 1;
            simulationPaused = false;
        }
        
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    public void PlayButton() {
        simulationPaused = false;
    }

    public void QuitButton() {
        Application.Quit();
    }
}
