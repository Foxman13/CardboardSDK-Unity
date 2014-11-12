﻿using UnityEngine;
using System.Collections;

public class ExampleCharacterController : MonoBehaviour {
  private static CardboardInput cardboard;

  public float speed = 1.0f;
  public float jumpSpeed = 1.0f;
  public float gravity = 1.0f;
  private Transform diveCameraTransform;
  private Vector3 moveDirection = Vector3.zero;
  private CharacterController controller;

	void Start () {
    // These can probably go somewhere better
	  Screen.sleepTimeout = SleepTimeout.NeverSleep;
    //Screen.orientation = ScreenOrientation.LandscapeLeft;

    cardboard = new CardboardInput();
    cardboard.OnMagnetDown += CardboardDown;
    cardboard.OnMagnetUp += CardboardUp;
    cardboard.OnMagnetClicked += CardboardClick;

    controller = GetComponent<CharacterController>();
    diveCameraTransform = this.transform.GetChild(0);
	}

  public void CardboardDown(object sender, CardboardEvent cardboardEvent) {
    ChangeSphereColor("SphereDown");
  }

  public void CardboardUp(object sender, CardboardEvent cardboardEvent) {
    ChangeSphereColor("SphereUp");
  }

  public void CardboardClick(object sender, CardboardEvent cardboardEvent) {
    ChangeSphereColor("SphereClick");

    RaycastHit hit;
    Ray ray = new Ray(diveCameraTransform.position, diveCameraTransform.forward);
    if (Physics.Raycast(ray, out hit, 10f)) {
      GameObject obj = hit.collider.gameObject;
      if (obj.layer == LayerMask.NameToLayer("Interactable")) {
        GameObject.Find("star_chimes").audio.Play();
        Object.Destroy(obj);
      }
    }
  }

  public void ChangeSphereColor(string name) {
    GameObject sphere = GameObject.Find(name);
    sphere.renderer.material.color = new Color(Random.value, Random.value, Random.value);
  }

	// Update is called once per frame
	void Update () {
    MoveCharacter();
	}

  void MoveCharacter() {
    cardboard.Update(Input.acceleration, Input.compass.rawVector);

    if (!cardboard.IsMagnetHeld()) {
      moveDirection = Vector3.zero;
    }
    else if (cardboard.SecondsMagnetHeld() > 0.5f) {
      moveDirection = diveCameraTransform.forward;
      moveDirection = transform.TransformDirection(moveDirection);
      moveDirection.y = 0;
      moveDirection *= speed;
    }

    moveDirection.y -= gravity * Time.deltaTime;
    controller.Move(moveDirection * Time.deltaTime);
  }
}