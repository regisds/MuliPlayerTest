﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public Transform tilePrefab;
	public Transform obstaclePrefab;
	public Transform navmeshFloor;
	public Transform navmeshMaskPrefab;

	public Vector2 mapSize;
	public Vector2 maxMapSize;
	[Range(0,1)]
	public float outlinePercent;
	[Range(0,1)]
	public float obstaclePercent;

	public float tileSize;
	List<Coord> allTileCoords;
	Queue<Coord> shuffleTileCoords;
	public int seed = 10;

	Coord mapCenter;


	void Start(){
		GenerateMap();
	}



	public void GenerateMap (){

		allTileCoords = new List<Coord> ();
		for (int x=0;x < mapSize.x; x ++){
			for (int y=0; y<mapSize.y; y ++){
				allTileCoords.Add(new Coord(x,y));
			}
		}

		shuffleTileCoords = new Queue<Coord> (Utility.ShuffleArray(allTileCoords.ToArray(), seed));
		mapCenter = new Coord ((int) mapSize.x / 2, (int)mapSize.y / 2);

		string holderName = "Generated Map";
		if (transform.Find (holderName)){
			DestroyImmediate(transform.Find(holderName).gameObject);
		}

		Transform mapHolder = new GameObject(holderName).transform;
		mapHolder.parent = transform;


		for (int x=0;x < mapSize.x; x ++){
			for (int y=0; y<mapSize.y; y ++){
				Vector3 tilePosition = CoordToPosition(x,y);
				Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right*90)) as Transform;
				newTile.localScale = Vector3.one * (1-outlinePercent) * tileSize;
				newTile.parent = mapHolder;
			}
		}

		bool [,] obstacleMap = new bool[(int)mapSize.x,(int)mapSize.y];


		int obstacleCount = (int)(mapSize.x * mapSize.y * obstaclePercent);
		int currentObstacleCount = 0;
		for (int i =0;i<obstacleCount;i++){
			Coord randomCoord = GetRandomCoord();
			obstacleMap[randomCoord.x, randomCoord.y] = true;
			currentObstacleCount ++;

			if (randomCoord != mapCenter && MapFullyAccessible(obstacleMap, currentObstacleCount)){
				Vector3 ObstaclePosition = CoordToPosition (randomCoord.x,randomCoord.y);


				Transform newObstacle = Instantiate (obstaclePrefab, ObstaclePosition + Vector3.up * .5f, Quaternion.identity) as Transform;
				newObstacle.parent = mapHolder;
				newObstacle.localScale = Vector3.one * (1-outlinePercent) * tileSize;
			}
			else{
				obstacleMap[randomCoord.x, randomCoord.y] = false;
				currentObstacleCount --;
			}
		}
		Transform maskLeft = Instantiate(navmeshMaskPrefab, Vector3.left * (mapSize.x + maxMapSize.x) / 4 * tileSize, Quaternion.identity)as Transform;
		maskLeft.parent = mapHolder;
		maskLeft.localScale = new Vector3((maxMapSize.x - mapSize.x)/2, 1, mapSize.y) * tileSize;

		Transform maskRight = Instantiate(navmeshMaskPrefab, Vector3.right * (mapSize.x + maxMapSize.x) / 4 * tileSize, Quaternion.identity)as Transform;
		maskRight.parent = mapHolder;
		maskRight.localScale = new Vector3((maxMapSize.x - mapSize.x)/2, 1, mapSize.y) * tileSize;

		Transform maskTop = Instantiate(navmeshMaskPrefab, Vector3.forward * (mapSize.y + maxMapSize.y) / 4 * tileSize, Quaternion.identity)as Transform;
		maskTop.parent = mapHolder;
		maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - mapSize.y)/2) * tileSize;

		Transform maskBotton = Instantiate(navmeshMaskPrefab, Vector3.back * (mapSize.y + maxMapSize.y) / 4 * tileSize, Quaternion.identity)as Transform;
		maskBotton.parent = mapHolder;
		maskBotton.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - mapSize.y)/2) * tileSize;

		navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;

	}

	bool MapFullyAccessible(bool[,] obstacleMap, int currentObstacleCount){
		bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
		Queue<Coord> queue = new Queue<Coord>();
		queue.Enqueue(mapCenter);
		mapFlags[mapCenter.x, mapCenter.y] = true;

		int accessibleTileCount = 1;

		while (queue.Count > 0) {
			Coord tile = queue.Dequeue();

			for (int x = -1; x<=1; x++){
				for (int y = -1; y<=1; y++){
					int neighbourX = tile.x + x;
					int neighbourY = tile.y + y;
					if (x==0 || y==0){
						if(neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1)){
							if (!mapFlags[neighbourX,neighbourY] && !obstacleMap[neighbourX,neighbourY]){
								mapFlags[neighbourX,neighbourY] = true;
								queue.Enqueue(new Coord(neighbourX,neighbourY));
								accessibleTileCount ++;
							}
						}
					}
				}
			}
		}
		int targetAccessibleTileCount = (int)(mapSize.x * mapSize.y - currentObstacleCount);
		return targetAccessibleTileCount == accessibleTileCount;


	}


	Vector3 CoordToPosition (int x, int y) {
		return new Vector3(-mapSize.x/2 +0.5f + x,0, -mapSize.y/2 + 0.5f + y) * tileSize;
	}




	public Coord GetRandomCoord (){
		Coord randomCoord = shuffleTileCoords.Dequeue();
		shuffleTileCoords.Enqueue(randomCoord);
		return randomCoord;
	}

	public struct Coord{
		public int x;
		public int y;
		public Coord(int _x, int _y){
			x=_x;
			y=_y;
		}

		public static bool operator ==(Coord c1, Coord c2){
			return c1.x == c2.x && c1.y ==c2.y;
		}
		public static bool operator !=(Coord c1, Coord c2){
			return !(c1 ==c2);
	}		
}
}