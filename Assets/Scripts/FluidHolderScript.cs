using UnityEngine;
using System.Collections;

public class FluidHolderScript : MonoBehaviour
{

    //We don't want to make a source every frame.
    public float timeBetweenSources = .1f;
    public float timeToNextSource = .1f;

    //Saving the outputs from multiple changes for one burst.
    //public float amountToOutput = 0.0f;
    //Used to figure out change proportion.
    //public float amountAtStartOfBurst = 0.0f;

    public float maxAmount;
    //public float currentAmount;

    public Solution solution;

    //When pouring out, this will be instantiated to do the pouring.
    public GameObject liquidSourcePrefab;

    // Use this for initialization
    void Start()
    {
        solution = ScriptableObject.CreateInstance<Solution>();
        //solution = new Solution();
    }

    // Update is called once per frame
    void Update()
    {
        timeToNextSource -= Time.deltaTime;

        //Don't want to spawn every frame. Save .1 seconds worth.
        if (timeToNextSource < 0f)
        {
            timeToNextSource = timeBetweenSources;

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

            Debug.Log(holdPercent);

            //If we spilled some liquid spawn particles and a hitbox to catch!
            if (holdPercent < solution.currentAmount / maxAmount)
            {
                Debug.Log("new it");
                timeToNextSource -= Time.deltaTime;

                float previousAmount = solution.currentAmount;

                //Clamp the actual amount to be between 0 and the rotated amount
                solution.currentAmount = Mathf.Clamp(solution.currentAmount, 0f, holdPercent * maxAmount);

                //The proportion lost.
                float differenceProportion = previousAmount / solution.currentAmount;

                //If went to infinity, make zero.
                if (differenceProportion > 1f )
                {
                    differenceProportion = 0;
                }

                //The actual amount lost.
                float differenceAmount = previousAmount - solution.currentAmount;

                Debug.Log("diff" + differenceProportion + " " + differenceAmount);


                //This source adds the liquid to containers below. Note, can add more accurate positioning, i.e. corner of container. 

                Vector3 spawnPos = transform.position + new Vector3(0, -2, 0);
                
                GameObject source = (GameObject)Instantiate(liquidSourcePrefab, spawnPos, Quaternion.identity);

                //The source will add the same solution, but different proportions, in fact the exact amount that was lost.
                source.GetComponent<SolutionSource>().solutionToAdd = ScriptableObject.CreateInstance<Solution>();
                source.GetComponent<SolutionSource>().solutionToAdd.init(solution);

                source.GetComponent<SolutionSource>().solutionToAdd.multiplyByFactor(1 - differenceProportion);//(amountAtStartOfBurst- solution.currentAmount) / maxAmount);
                source.GetComponentInChildren<ParticleSystem>().startColor = solution.getColor();
                source.GetComponent<SolutionSource>().parent = this;

                //Finally, for this container, decrease the amount of solution available.
                solution.multiplyByFactor(differenceProportion);

                //    amountToOutput = 0;
                //   amountAtStartOfBurst = previousAmount;
            }

            //else
            //{
            //    amountToOutput += differenceAmount;
            //}
            // Debug.Log(holdPercent);
        }

    }

    public void addToSolution(Solution other)
    {
        solution.addToSolution(other);
    }
}
