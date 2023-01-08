using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FluidSim : MonoBehaviour
{
    public ComputeShader FluidShader;
    public float Diffusion;
    public Camera camera;
    public RawImage image;
    private RenderTexture fluid;
    private RenderTexture fluid0;


    public RenderTexture result;
    public ComputeShader test;


    // Start is called before the first frame update
    void Start()
    {
        fluid = new RenderTexture(512, 512, 24);
        fluid.enableRandomWrite = true;
        fluid.Create();

        fluid0 = new RenderTexture(512, 512, 24);
        fluid0.enableRandomWrite = true;
        fluid0.Create();

        FluidShader.SetTexture(0, "Fluid", fluid);
        FluidShader.SetTexture(0, "Fluid0", fluid0);
        FluidShader.SetFloat("Diffusion", Diffusion);

        camera.targetTexture = fluid;

        //test
        result = new RenderTexture(512, 512, 24);
        result.enableRandomWrite = true;
        result.Create();
        test.SetTexture(0, "Result", result);

        image.texture = fluid;
    }

    // Update is called once per frame
    void Update()
    {
        RenderTexture temp = fluid0;
        fluid0 = fluid;
        fluid = temp;
        FluidShader.Dispatch(0, fluid.width / 8, fluid.height / 8, 1);

        //dispach
        test.Dispatch(0, result.width / 8, result.height / 8, 1);
    }
}
