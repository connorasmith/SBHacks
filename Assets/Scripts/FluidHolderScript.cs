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

    public GameObject sourceSpawnObject;

    public Vector3 start = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 end = new Vector3(0.0f, 1.0f, 0.0f);

	// Use this for initialization
	void Start () {
        solution = ScriptableObject.CreateInstance<Solution>();

        updateWaterImage();


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
        if ((solution.getAmount() / maxAmount - holdPercent) > .01f)
        {
            float previousAmount = solution.getAmount();

      float newAmount = solution.getAmount();
            
            //Clamp the actual amount to be between 0 and the rotated amount
            newAmount = Mathf.Clamp(solution.getAmount(), 0f, holdPercent * maxAmount);

      float newProportion = newAmount / maxAmount;

      Debug.Log(newAmount);

            //The proportion lost.
            //float differenceProportion = (previousAmount -newAmount) / maxAmount;

      //Debug.Log(differenceProportion); 

            //If went to infinity, make zero.
            /*if (differenceProportion > 1f)
            {
                differenceProportion = 0;
            }*/

            //The actual amount lost.
            float differenceAmount = previousAmount - newAmount;

            Vector3 spawnPos = sourceSpawnObject.GetComponent<Collider>().bounds.min;//transform.position + new Vector3(0,3 ,0);

           // Debug.Log(spawnPos);

            //Vector3 spawnPos = transform.position + new Vector3(0,3 ,0);

            //This source adds the liquid to containers below. Note, can add more accurate positioning, i.e. corner of container. 

            GameObject source = (GameObject)Instantiate(liquidSourcePrefab, spawnPos, Quaternion.identity);

            //The source will add the same solution, but different proportions, in fact the exact amount that was lost.
            Solution newSolution = ScriptableObject.CreateInstance<Solution>();
            newSolution.init(solution);

            source.GetComponent<SolutionSource>().solutionToAdd = newSolution;
            source.GetComponent<SolutionSource>().solutionToAdd.multiplyByFactor(1 - newProportion);
            source.GetComponent<SolutionSource>().creator = this;

            //Finally, for this container, decrease the amount of solution available.
            solution.multiplyByFactor(newProportion);
      Debug.Log(solution.getAmount());
            updateWaterImage();


            /*waterObject.transform.localPosition = Vector3.Lerp(start, end, (solution.currentAmount / maxAmount));
            
            if (solution.currentAmount == 0)
            {
                waterObject.SetActive(false);
            }

            else
            {
                waterObject.SetActive(true);

            }*/
            // Debug.Log(holdPercent);
        }

    }

    public void addToSolution(Solution other)
    {
        solution.addToSolution(other);

        //Debug.Log((solution.currentAmount / maxAmount));
        //waterObject.transform.localPosition = Vector3.Lerp(start, end, (solution.currentAmount / maxAmount));
        updateWaterImage();

        //Update color.
        Renderer rend = waterObject.GetComponent<Renderer>();
        rend.material.shader = Shader.Find("FX/Water");
        rend.material.SetColor("_RefrColor", solution.getColor());
    }

  public void changeLevel(float deltaLevel) {
    float differenceProportion = 1 / maxAmount;

    solution.multiplyByFactor(1 - differenceProportion);

    //solution.currentAmount += deltaLevel;

    //Debug.Log("rota" + (solution.currentAmount / maxAmount));
    //waterObject.transform.localPosition = Vector3.Lerp(start, end, (solution.currentAmount / maxAmount));
    updateWaterImage();
  }

    private void updateWaterImage()
    {
        waterObject.transform.localPosition = Vector3.Lerp(start, end, (solution.getAmount() / maxAmount));

        if (solution.getAmount() == 0)
        {
            waterObject.SetActive(false);
        }

        else
        {
            waterObject.SetActive(true);

        }
    }
}
