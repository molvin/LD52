using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidSim : MonoBehaviour
{
    public ComputeShader FluidShader;
    public float Diffusion;
    public Camera camera;
    private RenderTexture fluid;
    private RenderTexture fluid0;
    
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
    }

    // Update is called once per frame
    void Update()
    {

        FluidShader.Dispatch(0, fluid.width / 8, fluid.height / 8, 1);
        FluidShader.SetTexture(0, "Fluid", fluid0);
        FluidShader.SetTexture(0, "Fluid0", fluid);
    }
}
