using UnityEngine;

[ExecuteInEditMode]
public class PixelationEffect : MonoBehaviour
{
    public int pixelationAmount = 1;

    private int targetPixelationAmount;

    float startingTime = 0;
    public float duration = 1f;

    private void Start()
    {
        // Initialize the pixelation amount and animation timer
        targetPixelationAmount = pixelationAmount;

        pixelationAmount = 50;
        AnimatePixelationIn();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) AnimatePixelationIn();
        if (Input.GetKeyDown(KeyCode.Q)) AnimatePixelationOut();

        if(pixelationAmount != targetPixelationAmount) 
        {
            pixelationAmount = Mathf.RoundToInt(Mathf.Lerp(pixelationAmount, targetPixelationAmount, Time.time - startingTime / duration));
        }

    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (pixelationAmount == 0)
        {
            // Avoid division by zero
            Graphics.Blit(source, destination);
            return;
        }

        RenderTexture scaled = RenderTexture.GetTemporary(source.width / pixelationAmount, source.height / pixelationAmount);
        scaled.filterMode = FilterMode.Point;

        Graphics.Blit(source, scaled);
        Graphics.Blit(scaled, destination);

        RenderTexture.ReleaseTemporary(scaled);
    }

    // Function to animate the pixelation effect in
    public void AnimatePixelationIn()
    {
        targetPixelationAmount = 1;

        startingTime = Time.time;
    }

    // Function to animate the pixelation effect out
    public void AnimatePixelationOut()
    {
        targetPixelationAmount = 50;
        
        startingTime = Time.time;
    }
}
