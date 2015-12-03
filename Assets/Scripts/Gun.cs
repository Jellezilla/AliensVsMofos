using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Gun : MonoBehaviour {

	public enum FireMode {Auto, Burst, Single};
	public FireMode fireMode;

	public Texture gunLogo = null;

	// The spawn position of the projectile. Array, so we can shoot multiple projectiles at once.
	public Transform[] projectileSpawn;
	// Which projectiles are we shooting.
	public Projectile projectile;
	// Rate of fire of the weapon.
	public float msBetweenShots = 100;
	// The speed at which the projectile will leave the gun.
	public float muzzleVelocity = 35;
	public int burstCount;

	[Header("Effects")]
	public Transform shell;
	public Transform shellEjection;
	MuzzleFlash muzzleflash;
	float nextShotTime;

	bool triggerReleasedSinceLastShot;
	// How many shots we've fired in our current burst.
	int shotsRemainingInBurst;

	[Header("Recoil")]
	public Vector2 kickMinMax = new Vector2(.05f, .2f);
	public Vector2 recoilAngleMinMax = new Vector2(3, 5);
	public float recoilMoveSettleTime = .1f;
	public float recoilRotationSettleTime = .1f;

	Vector3 recoilSmoothDampVelocity;
	float recoilRotSmoothDampVelocity;
	// How much the gun rotates upwards when firing.
	float recoilAngle;

	AmmunitionVariables ammoVariables;
	public float currentAmmo;
	public float currentClip;
	public float maxAmmo;
	public float maxClip;
	AudioClip alienScream;
	private bool isReloading = false;
	public Text ammoText;
	

//	[HideInInspector]
//	public AmmoGUI gui;

	void Start() {
		muzzleflash = GetComponent<MuzzleFlash>();
		ammoVariables = GameObject.FindGameObjectWithTag ("Player").GetComponent<AmmunitionVariables>();
		shotsRemainingInBurst = burstCount;
		alienScream = Resources.Load ("alien-scream") as AudioClip;
		ammoText = GameObject.FindGameObjectWithTag("AmmoText").GetComponent<Text>() as Text;
		//Restock();

//		currentAmmoInMag = ammoPerMag;

//		if (gui) {
//			gui.SetAmmoInfo(totalAmmo, currentAmmoInMag);
//		}

		if(gameObject.tag == "Revolver") {
			currentAmmo = ammoVariables.revolverCurrentAmmo;
			currentClip = ammoVariables.revolverCurrentClip;
			maxAmmo = ammoVariables.revolverMaxAmmo;
			maxClip = ammoVariables.revolverMaxClip;
		} else if(gameObject.tag == "Rifle") {
			currentAmmo = ammoVariables.rifleCurrentAmmo;
			currentClip = ammoVariables.rifleCurrentClip;
			maxAmmo = ammoVariables.rifleMaxAmmo;
			maxClip = ammoVariables.rifleMaxClip;
		} else if(gameObject.tag == "Shotgun") {
			currentAmmo = ammoVariables.shotgunCurrentAmmo;
			currentClip = ammoVariables.shotgunCurrentClip;
			maxAmmo = ammoVariables.shotgunMaxAmmo;
			maxClip = ammoVariables.shotgunMaxClip;
		}  
	

	}

	void Update() {
		if(Input.GetKeyDown(KeyCode.R))
		{

			if (currentAmmo > 0) {
				Reload();
			}

		}
	}

	void LateUpdate() {
		// Animate recoil
		transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilMoveSettleTime);
		// Reducing the recoil angle back to zero over time.
		//recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);

		//transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;
	}


	//void SetCurrentGunType() {
//		if(gameObject.tag == "Revolver") {
//			currentAmmo = ammoVariables.revolverCurrentAmmo;
//			currentClip = ammoVariables.revolverCurrentClip;
//			maxAmmo = ammoVariables.revolverMaxAmmo;
//			maxClip = ammoVariables.revolverMaxClip;
//		} else if(gameObject.tag == "Rifle") {
//			currentAmmo = ammoVariables.rifleCurrentAmmo;
//			currentClip = ammoVariables.rifleCurrentClip;
//			maxAmmo = ammoVariables.rifleMaxAmmo;
//			maxClip = ammoVariables.rifleMaxClip;
//		} else if(gameObject.tag == "Shotgun") {
//			currentAmmo = ammoVariables.shotgunCurrentAmmo;
//			currentClip = ammoVariables.shotgunCurrentClip;
//			maxAmmo = ammoVariables.shotgunMaxAmmo;
//			maxClip = ammoVariables.shotgunMaxClip;
//		}  
//	}


	void Shoot() {

		//SetCurrentGunType();

		if (Time.time > nextShotTime && currentClip != 0 && isReloading == false) {

			// Burst fire logic
			if (fireMode == FireMode.Burst) {
				// Can't shoot if no shots are remaining in burst.
				if (shotsRemainingInBurst == 0) {
					return;
				}
				// If there are shots remaining in burst, then decrement "shotsRemainingInBurst".
				shotsRemainingInBurst--;
			}

			// Single fire logic
			else if (fireMode == FireMode.Single) {
				// Can't shoot if trigger hasn't been released since last shot.
				if (!triggerReleasedSinceLastShot) {
					return;
				}
			}

			for (int i = 0; i < projectileSpawn.Length; i++) {

				// Making sure we can't shoot a projectile every frame (creating delay).
				nextShotTime = Time.time + msBetweenShots / 1000;
		
				// Instantiate new projectile when we shoot.
				Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
				newProjectile.SetSpeed(muzzleVelocity);
			}

//			if (gui) {
//				gui.SetAmmoInfo(totalAmmo, currentAmmoInMag);
//			}

			//Debug.Log(currentAmmoInMag + "       " + totalAmmo);

			GetComponent<AudioSource>().Play();

			// Iinstantiate new shell when we shoot.
			Instantiate(shell, shellEjection.position, shellEjection.rotation);
			
			// Activate muzzleflash. "Activate" method declared in "MuzzleFlash" script.
			//muzzleflash.Activate();

			float kick = Random.Range(kickMinMax.x, kickMinMax.y);
			transform.localPosition -= Vector3.forward * kick;
			//recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
			//recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);


		
			currentClip -= 1;
			currentAmmo -= 1;

			if (gameObject.tag == "Revolver") {
				ammoVariables.revolverCurrentClip = currentClip;
				ammoVariables.revolverCurrentAmmo = currentAmmo;
				ammoText.text = "Revolver: " + currentClip + "/" + currentAmmo;

			}
			else if (gameObject.tag == "Rifle") {
				ammoVariables.rifleCurrentClip = currentClip;
				ammoVariables.rifleCurrentAmmo = currentAmmo;
				ammoText.text = "Rifle: " + currentClip + "/" + currentAmmo;

			}
			else if (gameObject.tag == "Shotgun") {
				ammoVariables.shotgunCurrentClip = currentClip;
				ammoVariables.shotgunCurrentAmmo = currentAmmo;
				ammoText.text = "Shotgun: " + currentClip + "/" + currentAmmo;
			}
			
			RaycastHit hit;
			PlayerScript playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
				//Vector3 direction = GetComponent<Rigidbody>().velocity.normalized;
			Debug.DrawRay(transform.position, transform.forward.normalized*100, Color.blue);
			if(Physics.Raycast(transform.position, transform.forward, out hit, 100)){	
				if (hit.collider.tag == "Enemy"){
					float finalDamage = playerScript.damage;
					int rand = Random.Range (1,100);
					if (rand <= playerScript.criticalChange){
						finalDamage *= playerScript.criticalMultiplier;
							//print("CRITICAL HIT");
					}
					EnemyAttributes enemyAttributes = hit.collider.gameObject.GetComponent<EnemyAttributes>();
					enemyAttributes.ApplyDamage((int)finalDamage);
					int random = Random.Range (0,100);
					if (random > 70)
						if (enemyAttributes.alive)
							enemyAttributes.transform.GetChild (0).GetComponent<AudioSource>().PlayOneShot(alienScream);

					enemyAttributes.TriggerHitAnimation(true);
				}
			}
			
		}
		
	}


	void Reload() {      

		//SetCurrentGunType();

		StartCoroutine(ReloadWait());

		if (currentClip < maxClip)  // is clip full?
		{
			if(currentAmmo >= maxClip) // Reload full clip
			{
				
				//GetComponent<AudioSource>().PlayOneShot(reloadSound);
				//gunAnim.animation.CrossFade("ReloadGun");
				
				StartCoroutine(ReloadWait());
				
			} else if(currentAmmo < maxClip) // reload not completely full clip
			{
				//GetComponent<AudioSource>().PlayOneShot (reloadSound);
				
				StartCoroutine(ReloadWait());
				
				if(currentAmmo + currentClip >= maxClip)
				{
					StartCoroutine(ReloadWait());
				} else {
					currentClip += currentAmmo;
					currentAmmo = 0;
				}
			}
		}

	}


//	public void Restock()
//	{
//		//SetCurrentGunType();
//		currentClip = 30;
//		maxClip = 30;
//		currentAmmo = 90;
//		maxAmmo = maxClip + currentAmmo;
//	}
	
	IEnumerator ReloadWait()
	{
		//SetCurrentGunType();
		Debug.Log("Reload Now");
		isReloading = true;
		yield return new WaitForSeconds(3.0F); 
		currentAmmo += currentClip; // add the remaining bullets in the clip to the total amount of bullets.
		currentClip = maxClip;      // put new bullets in current clip.
		currentAmmo -= maxClip;     // subtract the bullets just put in the clip, from the total amount of bullets.
		isReloading = false;

	}




	public void OnTriggerHold() {

		if(currentClip > 0)
		{
			Shoot();
		}

		triggerReleasedSinceLastShot = false;
	}

	public void OnTriggerRelease() {
		triggerReleasedSinceLastShot = true;
		shotsRemainingInBurst = burstCount;
	}
}
