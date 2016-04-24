using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using PicoGames.QuickRopes;
using PicoGames.Utilities;

public class RopeRendererSceneUI : MonoBehaviour
{
    public Color stoppedColor = Color.green;
    public Color playingColor = Color.red;

    public Slider radiusSlider;
    public Slider edgeCountSlider;
    public Slider edgeIndentSlider;
    public Slider edgeDetailSlider;

    public Slider strandCountSlider;
    public Slider strandOffsetSlider;
    public Slider strandTwistSlider;

    public Button playButton;

    public Text radiusText;
    public Text edgeCountText;
    public Text edgeIndentText;
    public Text edgeDetailText;
    public Text strandCountText;
    public Text strandOffsetText;
    public Text strandTwistText;
    public Text buttonText;

    public Material[] availMaterials;
    public QuickRope rope;
    public RopeRenderer rRender = null;

    void Start()
    {
        if(rRender == null)
            return;
        
        radiusSlider.value = rRender.Radius;
        edgeCountSlider.value = rRender.EdgeCount;
        edgeIndentSlider.value = rRender.EdgeIndent;
        edgeDetailSlider.value = rRender.EdgeDetail;
        strandCountSlider.value = rRender.StrandCount;
        strandOffsetSlider.value = rRender.StrandOffset;
        strandTwistSlider.value = rRender.StrandTwist;

        buttonText.text = "Press To " + ((rope.usePhysics) ? "Stop Physics" : "Play Physics");
        playButton.colors = new ColorBlock() 
        { 
            normalColor = (rope.usePhysics) ? playingColor : stoppedColor,
            highlightedColor = (rope.usePhysics) ? playingColor : stoppedColor,
            pressedColor = (rope.usePhysics) ? playingColor : stoppedColor,
            colorMultiplier = 1,
            fadeDuration = 0.2f
        };

        UpdateLabels();

        radiusSlider.onValueChanged.AddListener(UpdateRenderer);
        edgeCountSlider.onValueChanged.AddListener(UpdateRenderer);
        edgeIndentSlider.onValueChanged.AddListener(UpdateRenderer);
        edgeDetailSlider.onValueChanged.AddListener(UpdateRenderer);
        strandCountSlider.onValueChanged.AddListener(UpdateRenderer);
        strandOffsetSlider.onValueChanged.AddListener(UpdateRenderer);
        strandTwistSlider.onValueChanged.AddListener(UpdateRenderer);
    }
    
    public void TogglePhysics()
    {
        rope.usePhysics = !rope.usePhysics;
        
        buttonText.text = "Press To " + ((rope.usePhysics) ? "Stop Physics" : "Play Physics");

        playButton.colors = new ColorBlock()
        {
            normalColor = (rope.usePhysics) ? playingColor : stoppedColor,
            highlightedColor = (rope.usePhysics) ? playingColor : stoppedColor,
            pressedColor = (rope.usePhysics) ? playingColor : stoppedColor,
            colorMultiplier = 1,
            fadeDuration = 0.2f
        };

        rope.defaultColliderSettings.radius = rRender.Radius * 0.5f;

        rope.Generate();
    }

    void UpdateRenderer(float value)
    {
        rRender.Radius = radiusSlider.value;
        rRender.EdgeCount = (int)edgeCountSlider.value;
        rRender.EdgeIndent = edgeIndentSlider.value;
        rRender.EdgeDetail = (int)edgeDetailSlider.value;
        rRender.StrandCount = (int)strandCountSlider.value;
        rRender.StrandOffset = strandOffsetSlider.value;
        rRender.StrandTwist = strandTwistSlider.value;

        UpdateLabels();
    }

    void UpdateLabels()
    {
        radiusText.text = rRender.Radius.ToString("0.00");
        edgeCountText.text = rRender.EdgeCount.ToString();
        edgeIndentText.text = rRender.EdgeIndent.ToString("0.00");
        edgeDetailText.text = rRender.EdgeDetail.ToString();
        strandCountText.text = rRender.StrandCount.ToString();
        strandOffsetText.text = rRender.StrandOffset.ToString("0.00");
        strandTwistText.text = rRender.StrandTwist.ToString("0.00");
    }

    public void VisitAssetStore()
    {
        //Application.OpenURL("http://u3d.as/2Rh");
        Application.ExternalEval("window.open('http://u3d.as/2Rh','_blank')");
    }
    public void FadeToSingleStrand()
    {
        StopAllCoroutines();
        StartCoroutine(SmoothSetRope(.3f, 8, 1, 5f, 1, 0.75f, 0f));
    }
    public void FadeToHighPolyBraidedRope()
    {
        StopAllCoroutines();
        StartCoroutine(SmoothSetRope(0.2f, 12, 1, 5f, 6, 0.9f, 35f));
    }
    public void FadeToLowPolyBraidedRope()
    {
        StopAllCoroutines();
        StartCoroutine(SmoothSetRope(.3f, 6, 1, 5f, 3, 0.5f, 50f));
    }
    public void FadeToStarRope()
    {
        StopAllCoroutines();
        StartCoroutine(SmoothSetRope(.5f, 5, 2, 2.5f, 1, 0f, 15f));
    }
    public void FadeToFlowerRope()
    {
        StopAllCoroutines();
        StartCoroutine(SmoothSetRope(.5f, 7, 10, 2f, 1, 0f, 15f));
    }
    public void FadeToSmallCable()
    {
        StopAllCoroutines();
        StartCoroutine(SmoothSetRope(.15f, 8, 1, 5f, 4, 0.5f, 50f));
    }

    IEnumerator SmoothSetRope(float _radius, int _edgeCount, int _edgeDetail, float _edgeIndent, int _strandCount, float _strandOffset, float _strandTwist)
    {
        bool hasFinished = false;
        float edgeCountFloat = edgeCountSlider.value;
        float edgeDetailFloat = edgeDetailSlider.value;
        float strandCountFloat = strandCountSlider.value;

        while(!hasFinished)
        {
            radiusSlider.value = Mathf.MoveTowards(radiusSlider.value, _radius, Time.deltaTime * radiusSlider.maxValue * 0.5f);

            edgeCountFloat = Mathf.MoveTowards(edgeCountFloat, _edgeCount, Time.deltaTime * edgeCountSlider.maxValue * 0.5f);
            edgeDetailFloat = Mathf.MoveTowards(edgeDetailFloat, _edgeDetail, Time.deltaTime * edgeDetailSlider.maxValue * 0.5f);
            edgeIndentSlider.value = Mathf.MoveTowards(edgeIndentSlider.value, _edgeIndent, Time.deltaTime * edgeIndentSlider.maxValue * 0.5f);

            strandCountFloat = Mathf.MoveTowards(strandCountFloat, _strandCount, Time.deltaTime * strandCountSlider.maxValue * 0.5f);
            strandOffsetSlider.value = Mathf.MoveTowards(strandOffsetSlider.value, _strandOffset, Time.deltaTime * strandOffsetSlider.maxValue * 0.5f);
            strandTwistSlider.value = Mathf.MoveTowards(strandTwistSlider.value, _strandTwist, Time.deltaTime * strandTwistSlider.maxValue * 0.5f);

            edgeCountSlider.value = edgeCountFloat;
            edgeDetailSlider.value = edgeDetailFloat;
            strandCountSlider.value = strandCountFloat;

            if (radiusSlider.value == _radius &&
                edgeDetailSlider.value == _edgeDetail &&
                edgeCountSlider.value == _edgeCount &&
                edgeIndentSlider.value == _edgeIndent &&
                strandCountSlider.value == _strandCount &&
                strandOffsetSlider.value == _strandOffset &&
                strandTwistSlider.value == _strandTwist)
                hasFinished = true;

            yield return 0;
        }
    }
}
