﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour {

    public static PlayerController instance;

    public Animator animator;
    public List<GameObject> partyMembers;
    public Vector2[] endTiles;
    public EventNotification note;
    public Tilemap groundTilemap;
    public Tilemap obstaclesTilemap;
    public bool isMoving = false;
    public bool onCooldown = false;
    public bool onExit = false;
    public bool uiOpen = false;
    public bool inBattle = false;
    public bool isTransitioning = false;
    public string areaTransitionName;
    public bool noteCooldown;

    private int goldKeyCount;
    private float moveTime = .2f;
    private const float COOLDOWN = .3f;
    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit;

    private static StandaloneInputModuleV2 currentInput;
    private StandaloneInputModuleV2 CurrentInput {
        get {
            if (currentInput == null) {
                currentInput = EventSystem.current.currentInputModule as StandaloneInputModuleV2;
                if (currentInput == null) {
                    Debug.LogError("Missing StandaloneInputModuleV2.");
                    // some error handling
                }
            }
            return currentInput;
        }
    }
    void Awake () {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        goldKeyCount = GameManager.instance.currentGoldKeys;

        for(var i = 0; i < GameManager.instance.heroes.Length; i++) {
            var hero = GameManager.instance.heroes[i];
            if (i == 0) { // party leader
                hero.GetComponent<SpriteRenderer>().enabled = false;
                continue;
            }
            partyMembers.Add(hero.gameObject);
            hero.gameObject.transform.position = this.gameObject.transform.position;
        }

        foreach(var member in partyMembers) {
            member.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
        }

    }

    void Update() {
        if (isMoving || isTransitioning || onCooldown || onExit || uiOpen || inBattle ) return;

        if (Input.GetMouseButton(0)) {
            var tmp = FindObjectOfType<EventSystem>();
            var tmp2 = tmp.IsPointerOverGameObject() || tmp.IsPointerOverGameObject(0);
            if (tmp2){
                var currentGO = CurrentInput.GameObjectUnderPointer();
                var currentGOtouch = CurrentInput.GameObjectUnderPointer(0);
                var layer = 0;

                if (currentGO == null) {
                    layer = currentGOtouch.layer;
                } else {
                    layer = currentGO.layer;
                }
                if (layer == 8) { // Clickables Layer
                    // if (noteCooldown) return;
                    // noteCooldown = true;
                    // var clickable = currentGO.GetComponent<Clickable>();
                    // StatusManager.instance.Notification(clickable.GetInfo());
                    // return;
                } else if (layer == 5 || layer == 2) { // UI & Ignore Raycast Layers
                    return;
                }
            }

            var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            var horizontal = 0;
            var variance = .75f;
            var deltaX = position.x - transform.position.x;
            if (deltaX > variance) {
                horizontal = 1;
            } else if (deltaX < -variance) {
                horizontal = -1;
            }

            var vertical = 0;
            var deltaY = position.y - transform.position.y;
            if (deltaY > variance) {
                vertical = 1;
            } else if (deltaY < -variance) {
                vertical = -1;
            }

            //We can't go in both directions at the same time
            var comparison = Mathf.Abs(deltaX) - Mathf.Abs(deltaY);
            if ( Mathf.Abs(deltaX) > Mathf.Abs(deltaY) ) {
                vertical = 0;
            } else if (Mathf.Abs(deltaY) > Mathf.Abs(deltaX)) {
                horizontal = 0;
            }

            //If there's a direction, we are trying to move.
            if (horizontal != 0 || vertical != 0) {
                animator.SetFloat("moveX", horizontal);
                animator.SetFloat("moveY", vertical);
                Move(horizontal, vertical);
            }

        }
    }

	private void FixedUpdate () {
        //We do nothing if the player is still moving.
        if (isMoving || isTransitioning || onCooldown || onExit || uiOpen || inBattle ) return;

        //To store move directions.
        int horizontal = 0;
        int vertical = 0;

        //To get move directions
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        //We can't go in both directions at the same time
        if ( horizontal != 0 ) {
            vertical = 0;
        }
      
        //If there's a direction, we are trying to move.
        if (horizontal != 0 || vertical != 0) {
            animator.SetFloat("moveX", horizontal);
            animator.SetFloat("moveY", vertical);
            Move(horizontal, vertical);
        }
	}

    private void Move(int xDir, int yDir, float cooldown = COOLDOWN) {
        if (isMoving || onCooldown || onExit || uiOpen || inBattle ) return;
        Vector2 startTile = transform.position;
        Vector2 endTile = startTile + new Vector2(xDir, yDir);
        // Debug.Log(startTile + " " + endTile);

        bool isOnGround = GetTile(groundTilemap, startTile) != null; //If the player is on the ground
        bool hasGroundTile = GetTile(groundTilemap, endTile) != null; //If target Tile has a ground
        bool hasObstacleTile = GetTile(obstaclesTilemap, endTile) != null; //if target Tile has an obstacle

        //If the player starts their movement from a ground tile.
        if (isOnGround) {
            //If the front tile is a walkable ground tile, the player moves here.
            if (hasGroundTile && !hasObstacleTile) {
                if (CollisionCheck(endTile)){
                    cooldown *= .66f;
                    PlayGrassSound();

                    endTiles[0] = startTile;
                    for (var i = 1; i < partyMembers.Count; i++) {
                        endTiles[i] = (partyMembers[i-1].transform.position);
                    }

                    isMoving = true;
                    StartCoroutine(SmoothMovement(this.gameObject, endTile));
    
                    for (var i = 0; i < partyMembers.Count; i++) {
                        StartCoroutine(SmoothMovement(partyMembers[i], endTiles[i]));
                    }
                }
                else {
                    StartCoroutine(BlockedMovement(endTile));
                }
            } else {
                if (!isMoving) {
                    StartCoroutine(BlockedMovement(endTile));
                }
            }
        }
        StartCoroutine(ActionCooldown(cooldown));
    }

    private IEnumerator SmoothMovement(GameObject unit, Vector3 endTile) {
        var horizontal = Mathf.RoundToInt(endTile.x - unit.transform.position.x);
        var vertical = Mathf.RoundToInt(endTile.y - unit.transform.position.y);

        var anim = unit.GetComponent<Animator>();
        anim.SetFloat("moveX", horizontal);
        anim.SetFloat("moveY", vertical);

        float sqrRemainingDistance = (unit.transform.position - endTile).sqrMagnitude;
        float inverseMoveTime = 1 / moveTime;

        while (sqrRemainingDistance > float.Epsilon) {
            Vector3 newPosition = Vector3.MoveTowards(unit.transform.position, endTile, inverseMoveTime * Time.deltaTime);
            unit.transform.position = newPosition;
            sqrRemainingDistance = (unit.transform.position - endTile).sqrMagnitude;

            yield return null;
        }
        isMoving = false;
    }

    //Blocked animation
    private IEnumerator BlockedMovement(Vector3 end) {
        isMoving = true;

        AudioManager.instance.PlaySfx("blocked");

        Vector3 originalPos = transform.position;

        end = transform.position + ((end - transform.position) / 5);
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        float inverseMoveTime = (1 / (moveTime * 3f) );

        while (sqrRemainingDistance > float.Epsilon) {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, end, inverseMoveTime * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            yield return null;
        }

        sqrRemainingDistance = (transform.position - originalPos).sqrMagnitude;
        while (sqrRemainingDistance > float.Epsilon) {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, originalPos, inverseMoveTime * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (transform.position - originalPos).sqrMagnitude;

            yield return null;
        }

        //The chest disable the sound so it doesn't overlap with this one, so 'blocked' has been muted; we restore it.
        AudioManager.instance.UnMute("blocked");

        // if ( AudioManager.instance != null && AudioManager.instance.PlaySfx("blocked").source.mute ) {
        //     AudioManager.instance.PlaySfx("blocked").source.Stop();
        //     AudioManager.instance.PlaySfx("blocked").source.mute = false;
        // }

        isMoving = false;
    }

    // public IEnumerator Teleport(PassageWay Teleporter, float aTime)
    // {

    //     //If the teleporter is in use, abort
    //     if (Teleporter.isTeleporting) yield break;

    //     //We wait for any other movement coroutines to finish before starting this one.
    //     while (isMoving) yield return null;

    //     isMoving = true;

    //     Debug.Log("Teleporting from " + name);

    //     //We set both teleporters as "In Use"
    //     Teleporter.setTeleportersAvailability(true);
    //     //we prevent the player from moving while teleporting

    //     float alpha = GetComponent<Renderer>().material.color.a;

    //     //The character disappear
    //     for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
    //     {
    //         Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, 0f, t));
    //         GetComponent<Renderer>().material.color = newColor;
    //         yield return null;
    //     }

    //     //Now me teleport the player
    //     transform.position = Teleporter.exitPos();

    //     //The character fades back to reality 
    //     for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime) {
    //         Color newColor = new Color(1, 1, 1, Mathf.Lerp(0f, alpha, t));
    //         GetComponent<Renderer>().material.color = newColor;
    //         yield return null;
    //     }

    //     //We allow the player to move again
    //     isMoving = false;
    //     //We set both teleporter as "Available"
    //     Teleporter.setTeleportersAvailability(false);
    // }

    private IEnumerator ActionCooldown(float cooldown) {
        onCooldown = true;

        while ( cooldown > 0f ) {
            cooldown -= Time.deltaTime;
            yield return null;
        }
        onCooldown = false;
    }

    // IEnumerator Teleport(Vector2 targetPos, float aTime)
    // {
    //     float alpha = GetComponent<Renderer>().material.color.a;

    //     //The character disappear
    //     for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
    //     {
    //         Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, 0f, t));
    //         GetComponent<Renderer>().material.color = newColor;
    //         yield return null;
    //     }

    //     //Now me teleport it
    //     transform.position = targetPos;

    //     //Now it fades back to reality 
    //     for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime) {
    //         Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, 1f, t));
    //         GetComponent<Renderer>().material.color = newColor;
    //         yield return null;
    //     }
    // }

    //A method that handle doors : Return true if you can move on the tile, false otherwise. 
    //If the door can be opened, opens it.
    //TO ADD : Levered Doors.
    private bool CollisionCheck(Vector2 targetCell) {
        Collider2D coll = WhatsThere(targetCell);

        //No obstacle, we can walk there
        if (coll == null) {
            return true;
        }

        //If there's a door in front of the character
        if (coll.tag == "Door") {
            Door door = coll.gameObject.GetComponent<Door>();
            //If the door is locked and we can unlock it
            if (door.isLocked) {
                if (goldKeyCount > 0) {
                    AdjustKeys(-1);
                    door.Unlock();
                }
                return false;
            } else if (door.isClosed) {
                door.Open();
                return true;
            } else if (!door.isClosed) //If it's already opened
                return true;
            else //If it's locked.
                return false;
        }
        // //if there's a levered door in front of the character.
        // else if (coll.tag == "LeveredDoor")
        // {
        //     Debug.Log("LeveredDoor detected!");
        //     LeveredDoor door = coll.gameObject.GetComponent<LeveredDoor>();
        //     //If the door is open
        //     if (door.isOpen)
        //         return true;
        //     //If the door is close.
        //     else
        //         return false;
        // }
        else if (coll.tag == "Chest") {
            Chest chest = coll.gameObject.GetComponent<Chest>();
            
            // if (!chest.isUnlocked) {
            //     chest.Unlock();
            if (!chest.isOpened) {
                if (chest.isLocked) {
                    if (GameManager.instance.currentGoldKeys > 0) {
                        AdjustKeys(-1);
                        chest.Unlock();
                    } else {
                        GameMenu.instance.Notification("You need a key.");
                    }
                } else if (!chest.isLocked) {
                    chest.Open();
                }
            } else if (!chest.isLooted) {
                chest.Loot();
            }

            //We interact with the chest, but can't move there, so we return false.
            return false;
        } else if (coll.tag == "Key") {
            return true;
        } else if (coll.tag == "Transition") {
            return true;
        } else if (coll.tag == "Enemy") {
            inBattle = true;
            return false;
        } else
            return false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        //Debug.Log("Something touched!");
        //If we collided with the exit, we load the next level in two seconds.
        if ( other.tag == "Exit") {
            Debug.Log("Sortie touché!");
            if (AudioManager.instance != null) {
                AudioManager.instance.PlaySfx("victory");
            }
            onExit = true; //Prevent the player from moving.
            //Invoke("NextLevel", 1f);
            //enabled = false;
        }
        else if ( other.tag == "Key" ) {
            Debug.Log("Key picked up!");
            GameMenu.instance.LootNotification("othrGoldKey");
            AdjustKeys(1);
            if (AudioManager.instance != null)
                AudioManager.instance.PlaySfx("loot");
            other.gameObject.SetActive(false);
        }
        else if ( other.tag == "Transition" ) {
            // other.gameObject.SetActive(false);

            var xPos = 0f;
            var yPos = 0f;
            var xDir = 0;
            var yDir = 0;

            if (other.transform.position.x - transform.position.x < 0f) {
                xPos = -25f;
                xDir = -1;
            } else if (other.transform.position.x - transform.position.x > 0f) {
                xPos = 25f;
                xDir = 1;
            } else if (other.transform.position.y - transform.position.y < 0f) {
                yPos = -17f;
                yDir = -1;
            } else {
                yPos = 17f;
                yDir = 1;
            }

            var endPos = CameraController.instance.transform.position + new Vector3(xPos, yPos, 0f);
            StartCoroutine(CameraPositionLerpTo(endPos, 0.5f, xDir, yDir));
        }
    }

    public void PlayGrassSound() {
    if (AudioManager.instance != null) {
            AudioManager.instance.PlaySfx("grass");
        }
    }
    public void PlayBridgeSound() {
        if ( AudioManager.instance != null ) {
            AudioManager.instance.PlaySfx("bridge");
        }
    }

    public Collider2D WhatsThere(Vector2 targetPos) {
        RaycastHit2D hit;
        hit = Physics2D.Linecast(targetPos, targetPos);
        return hit.collider;
    }

    // private void NextLevel() {
    //     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
    // }

    private TileBase GetTile(Tilemap tilemap, Vector2 tileWorldPos) {
        return tilemap.GetTile(tilemap.WorldToCell(tileWorldPos));
    }

    private bool HasTile(Tilemap tilemap, Vector2 cellWorldPos) {
        return tilemap.HasTile(tilemap.WorldToCell(cellWorldPos));
    }

    public void SetBounds(Vector3 bottomLeft, Vector3 topRight) {
        bottomLeftLimit = bottomLeft + new Vector3(.5f, .8f, 0);
        topRightLimit = topRight + new Vector3(-.5f, -.8f, 0);
    }

    public void AdjustKeys(int amount) {
        GameManager.instance.AdjustKeys(amount);
    }

    public IEnumerator CameraPositionLerpTo(Vector3 endPos, float _duration, int xDir, int yDir) {
        isTransitioning = true;
        float elapsedTime = 0.0f;
        var startPos = CameraController.instance.transform.position;
        while (CameraController.instance.transform.position != endPos) {
            CameraController.instance.transform.position = Vector3.Lerp(startPos, endPos, (elapsedTime / _duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForEndOfFrame();
        Move(xDir, yDir, .35f);
        isTransitioning = false;
    }
}