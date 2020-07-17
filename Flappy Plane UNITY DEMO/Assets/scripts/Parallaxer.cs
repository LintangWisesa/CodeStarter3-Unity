using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxer : MonoBehaviour {

	class PoolObject{

		public Transform transform;
		public bool inUse;
		public PoolObject (Transform t){transform =t;}
		public void Use(){
			inUse = true;
		}

		public void Dispose(){
		
			inUse = false;
		}
	}

	[System.Serializable]//inspectorda struct gözüksün diye
	public struct YSpawnRange{//Woodların Ysi değişeceği için min ve max Y değerleri olması gerekir.
		public float min;
		public float max;

	}

	public GameObject Prefab;//hangi tip prefab doğacak
	public int poolSize;//Ne kadar prefabin doğması gerekiyor ya da yeterlidir.(kaç wood kaç stars kaç clouds)
	public float shiftSpeed;//nesnelerin hızı
	public float spawnRate;//nesneler ne sıklıkla doğuyor

	public YSpawnRange ySpawnRange;
	public Vector3 defaultSpawnPos;//nesnelerin varsayılan doğma pozisyonu
	public bool spawnImmediate;//doğacak nesnelerimizin başta ayarlanması için gerekli??????
	public Vector3 immediateSpawnPos;
	public Vector2 targetAspectsRatio;//Aspect Ratiosu büyük olan ekranlı cihazlar için (ipad).
	//nesnelerin doğarken veya yok olurken ekranda gözükmemesi için gerekli.Buna göre varsayılan ve anlık doğma pozisyonlarımız ile ilişkili

	float spawnTimer;
	float targetAspect;
	PoolObject[] poolObjects;
	GameManager game;

	void Awake(){
	
		Configure ();
	}

	void Start(){
		game = GameManager.Instance;
	
	}

	void OnEnable(){
		GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
	}
	void OnDisable(){
		GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
	}
	void OnGameOverConfirmed(){
		for (int i = 0; i < poolObjects.Length; i++) {
			poolObjects [i].Dispose ();
			poolObjects[i].transform.position=Vector3.one*1000;
		
		}		
		if (spawnImmediate) {
			SpawnImmediate ();

		}
	
	}
	void Update(){
		if (game.GameOver)
			return;
		
		Shift ();
		spawnTimer += Time.deltaTime;

		if (spawnTimer > spawnRate) {
			Spawn ();
			spawnTimer = 0;
		}
	}

	void Configure(){

		targetAspect = targetAspectsRatio.x / targetAspectsRatio.y;
		poolObjects = new PoolObject [poolSize];
		for (int i = 0; i < poolObjects.Length; i++) {

			GameObject go = Instantiate (Prefab) as GameObject;//Prefabi klonlamak için kulllanılan metod
			Transform t=go.transform;
			t.SetParent (transform);//t script hangi objeye bağlıysa onun transformunu alıyor
			t.position=Vector3.one*1000;
			poolObjects [i] = new PoolObject (t);
		
		}
		if (spawnImmediate) {
			SpawnImmediate ();
		
		}
	}

	void Spawn(){

		Transform t = GetPoolObject ();
		if (t == null)//herhangi uygun bir nesne bulamadı.poolSize çok küçük.
			return;
		Vector3 pos = Vector3.zero;
		pos.x = (defaultSpawnPos.x*Camera.main.aspect)/targetAspect;
		pos.y = Random.Range (ySpawnRange.min,ySpawnRange.max);
		t.position = pos;
	}

	void SpawnImmediate(){
		Transform t = GetPoolObject ();
		if (t == null)//herhangi uygun bir nesne bulamadı.poolSize çok küçük.
			return;
		Vector3 pos = Vector3.zero;
		pos.x = (immediateSpawnPos.x*Camera.main.aspect)/targetAspect;
		pos.y = Random.Range (ySpawnRange.min,ySpawnRange.max);
		t.position = pos;
		Spawn ();
	}

	void Shift(){
		for (int i = 0; i < poolObjects.Length ; i++) {
		
			poolObjects [i].transform.position+= -Vector3.right * shiftSpeed * Time.deltaTime;
			CheckDisposeObject (poolObjects [i]);
		
		}
	}

	void CheckDisposeObject(PoolObject poolObject){//nesnemiz ekranın dışına çıkmış mı
		if(poolObject.transform.position.x<(-defaultSpawnPos.x*Camera.main.aspect)/targetAspect){
			poolObject.Dispose ();
			poolObject.transform.position = Vector3.one * 1000;//nesne artık kullanılmadığı için çok uzaklara gönderilir.
		}
	
	}

	Transform GetPoolObject(){//uygun durumda olan objeyi alabilmemiz için poolObjects dizimizden
		for(int i=0;i<poolObjects.Length;i++){

			if (!poolObjects [i].inUse) {
				poolObjects [i].Use ();
				return poolObjects [i].transform;
			}
		}
		return null;
	}

}
