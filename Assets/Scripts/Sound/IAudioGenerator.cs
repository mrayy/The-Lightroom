using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAudioGenerator {

	int SamplesCount();
	float Sample ();
}
