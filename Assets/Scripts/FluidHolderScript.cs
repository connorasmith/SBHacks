using UnityEngine;
using System.Collections;

public class FluidHolderScript : MonoBehaviour {

    
    public float maxAmount;
    //public float currentAmount;

    public Solution solution;

    //When pouring out, this will be instantiated to do the pouring.
    public GameObject liquidSourcePrefab;

    //A visible water object for when there's stuff in this holder.
    public GameObject waterObject;

    public Vector3 start = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 end = new Vector3(0.0f, 1.0f, 0.0f);

	// Use this for initialization
	void Start () {
        solution = ScriptableObject.CreateInstance<Solution>();
        //solution = new Solution();
	}
	
	// Update is called once per frame
	void Update () {
        //If tilted, then pour portion of liquid out based on percentage.

        float xAngle = transform.rotation.eulerAngles.x;
        float zAngle = transform.rotation.eulerAngles.z;

        //Normalize angles to 0-180 range. Neg doesn't matter
        if (xAngle > 180)
        {
            xAngle = Mathf.Abs(xAngle - 360);
        }

        //Normalize angles to 0-180 range. Neg doesn't matter
        if (zAngle > 180)
        {
            zAngle = Mathf.Abs(zAngle - 360);
        }

        float largestAngle = Mathf.Max(xAngle, zAngle);

        //What percent of the liquid can we hold?
        float holdPercent = Mathf.Clamp((90 - largestAngle) / 90, 0f, 1f);

        //If we spilled some liquid spawn particles and a hitbox to catch!
        if (holdPercent < solution.currentAmount / maxAmount)
        {
            float previousAmount = solution.currentAmount;

            //Clamp the actual amount to be between 0 and the rotated amount
            solution.currentAmount = Mathf.Clamp(solution.currentAmount, 0f, holdPercent * maxAmount);

            //The proportion lost.
            float differenceProportion = previousAmount / solution.currentAmount;

            //The actual amount lost.
            float differenceAmount = previousAmount - solution.currentAmount;

            //This source adds the liquid to containers below. Note, can add more accurate positioning, i.e. corner of container. 

            GameObject source = (GameObject)Instantiate(liquidSourcePrefab, transform.position, Quaternion.identity);

            //The source will add the same solution, but different proportions, in fact the exact amount that was lost.
            source.GetComponent<SolutionSource>().solutionToAdd = new Solution(solution);
            source.GetComponent<SolutionSource>().solutionToAdd.multiplyByFactor(1- differenceProportion);

            //Finally, for this container, decrease the amount of solution available.
            solution.multiplyByFactor(differenceProportion);

            waterObject.transform.localPosition = Vector3.Lerp(start, end, (solution.currentAmount / maxAmount));
            
            // Debug.Log(holdPercent);
        }

    }

    public void addToSolution(Solution other)
    {
        solution.addToSolution(other);

        waterObject.transform.localPosition = Vector3.Lerp(start, end, (solution.currentAmount / maxAmount));

        //Update color.
        Renderer rend = waterObject.GetComponent<Renderer>();
        rend.material.shader = Shader.Find("FX/Water");
        rend.material.SetColor("_RefrColor", solution.getColor());
    }
}
