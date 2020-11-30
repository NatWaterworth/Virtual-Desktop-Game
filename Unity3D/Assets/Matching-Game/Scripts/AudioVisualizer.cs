using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualizer : MonoBehaviour
{
    AudioSource audioSource;
    public static float[] samples = new float[512];
    public static float[] frequencyBand = new float[8];
    public static float[] bandBuffer = new float[8];
    float[] bufferDecrease = new float[8];

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (audioSource != null)
        {
            GetSpectrumAudioSource();
            MakeFrequencyBands();
            BandBuffer();
        }
    }

    public void SetAudioSource(AudioSource source)
    {
        audioSource = source;
    }

    void GetSpectrumAudioSource()
    {
        //Compacts all audio samples 20k into 512 samples, channel  = 0, Blackman window to reduce leakage of spectrum data
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
    }

    //returns an array of the band amplitudes
    public float[] GetBandAmplitudes()
    {
        return frequencyBand;
    }

    //Returns the max amplitude out of bands
    public float GetMaxBandAmpltiude()
    {
        float max = 0;
        foreach(float amp in frequencyBand)
        {
            max = Mathf.Max(amp, max);
        }
        return max;
    }

    void BandBuffer()
    {
        for(int g = 0; g < 8; g++)
        {
            if (frequencyBand[g] > bandBuffer[g])
            {         
                bandBuffer[g] = frequencyBand[g];
                bufferDecrease[g] = 0.005f;
            }
            if (frequencyBand[g] < bandBuffer[g])
            {
                bandBuffer[g] -= bufferDecrease[g];
                bufferDecrease[g] *= 1.2f;
            }
        }
    }

    void MakeFrequencyBands()
    {
        //we have a frequency range of 22050. For 512 samples. 43Hz per sample.
        //split into 8 ranges.
        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7)
            {
                sampleCount += 2;
            }

            for(int j = 0; j < sampleCount; j++)
            {
                average += samples[count] * (count + 1);
                count++;
            }

            average /= count;

            frequencyBand[i] = average * 10;
        }
    }
}
