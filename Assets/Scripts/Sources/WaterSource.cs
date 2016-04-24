using UnityEngine;
using System.Collections;

public class WaterSource : SolutionSource {

	// Use this for initialization
	void Start () {
        Liquid water = ScriptableObject.CreateInstance<Liquid>(); //5, Color.white, LiquidType.Water);
        water.init(5, Color.white, LiquidType.Water);


        Liquid sketchyWater = ScriptableObject.CreateInstance<Liquid>();
        sketchyWater.init(10, Color.red, LiquidType.Water);

        Solution solution = ScriptableObject.CreateInstance<Solution>();
        solution.addToSolution(water);
        solution.addToSolution(sketchyWater);

        solutionToAdd = solution;
    }

    // Update is called once per frame
    void Update () {
	
	}
}
