![FingerTheBalance_gif1](https://github.com/user-attachments/assets/d732e1d6-32f4-437d-89af-fa19f8cf4799)
![FingerTheBalance_gif2](https://github.com/user-attachments/assets/dedbd6bc-70b2-41cc-8279-2de76b52ed33)


## 🔎개요

> 낙하하는 물체를 손으로 잡아서 제한시간동안 떨어트리지 않고 버티는 게임입니다.
> 

## 🛠️사용기술

- Unity 2D
- C#

## 🎮주요기능


### 🪝 **잡기 기능**

- 물체와 손이 접촉하면 hinge joint의 연결 Anchor를 접촉지점으로 수정해서 손에 붙어 있을 수 있게 했다.
    
    ```csharp
    private void OnCollisionEnter2D(Collision2D collision)
    {
    	Vector2 contactPoint = collision.GetContact(0).point;
    	connectedAncor.transform.position = contactPoint;
    	// hinge joint의 Anchor를 접촉지점으로 옮김
    	joint.anchor = connectedAncor.transform.localPosition;
    	joint.enabled = true;
    	isConnected = true;
    
    	SoundManager.Instance.Play("Tup");
    	Finger finger = collision.gameObject.GetComponent<Finger>();
    	finger.ConnectPivot(contactPoint);
    	// hinge joint와 연결된 anchor도 접촉지점으로 옮김
    	joint.connectedAnchor = finger.GetConnectedAncor();
    }
    ```
    

### 🎐 스테이지 세팅

- 처음에는 플레이 중에 Hinge joint의 Connected Rigidbody에 손을 연결하려고 했었다. 하지만 플레이 중에는 연결이 불가능했고 어쩔 수 없이 씬에서 연결된 상태에서 다른 방법을 찾았다.
- FallingObject에 Rigidbody와 Hinge joint 컴포넌트를 두고 자식 오브젝트로 collider를 가진 오브젝트를 생성하는 방식으로 해결했다.
    <img width="297" height="84" alt="image" src="https://github.com/user-attachments/assets/7413b488-20bd-4dd0-9e49-8632063edfa4" />

    
    ```csharp
    public void InitStage(Vector3 fallingObjectStartPosition, GameObject fallingObject, float gravityScale)
    {
    	// 손과 접촉하기 전까지 연결상태를 끊어둔다.
    	isConnected = false;
    	joint.enabled = false;
    
    	rigid.gravityScale = 0;
    	transform.rotation = Quaternion.identity;
    
    	this.gravityScale = gravityScale;
    	transform.position = fallingObjectStartPosition;
    
    	// 이전에 존재하던 오브젝트를 삭제하고
    	foreach (Transform child in fallingObjectTransform)
    		Destroy(child.gameObject);
    
    	// 새로운 오브젝트를 fallingObjectTransform에 생성한다.
    	Instantiate(fallingObject, transform.position, fallingObject.transform.rotation, fallingObjectTransform);
    	// 새로운 오브젝트기준으로 무게중심을 재설정 한다.
    	centerOfMass.localPosition = rigid.centerOfMass;
    }
    ```
    

### 🌐 **중력 적용**

- 손으로 물체를 받은 이후 Rigidbody의 중력을 그대로 사용했더니 가속도가 남아 있어서 물체가 반대로 기울어지는 상황이 있었다.
- 이 문제를 해결하기 위해 중력을 적용한 듯한 회전을 기울어진 정도에 따라 회전속도를 조절했다.
    
    ```csharp
    private void FixedUpdate()
    {
    	if (joint == null || !joint.enabled)
    		return;
    	// 회전하는 속도는 손과 접촉지점의 x값과 무게중심의 x값 사이의 차이를 이용했다.
    	float speed = centerOfMass.position.x - connectedAncor.transform.position.x;
    	rigid.angularVelocity = -speed * gravity * gravityScale;
    }
    ```
    
- 기울어진 정도를 구하기 위해서 무게중심을 구하고 transform만 가진 오브젝트로 시각화했다.

  <img width="462" height="430" alt="image" src="https://github.com/user-attachments/assets/5a658553-5f96-4abf-b19e-f6facc67759d" />


### 🔲 **연출**

- 모든 UI 연출은 애니메이션으로 만들었다.
- 순차적인 연출을 위해 애니메이션의 종료시점을 알기 위해서 작성한 함수다.
    
    ```csharp
    IEnumerator EndOfAnimation(string animationName)
    {
    		//애니메이션이 종료되면 함수를 리턴한다.
        while (!EndAnimationDone(animationName))
        {
            yield return null;
    		}
    }
    
    bool EndAnimationDone(string animationName)
    {
    		// 현재 애니메이션의 이름이 animationName이고 애니메이션이 99% 진행되면 true
        return anim.GetCurrentAnimatorStateInfo(0).IsName(animationName) &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f;
    }
    ```
    
- 연출을 순차적으로 진행하기 위해 코루틴을 사용해서 만들었다.
    
    EX) 클리어 연출
    
    ```csharp
    public void ClearStage()
    {
    	StartCoroutine(ClearProcess());
    }
    
    IEnumerator ClearProcess()
    {
    	currentUI = CurrentUI.CHANGING;
    	// 손과 떨어지는 오브젝트의 움직임을 멈춘다.
    	isPlaying = false;
    	fallingObject.FinishStage();
    	// 클리어 UI세팅
    	stageManager.SetClearMessage(currentStage);
    	currentStage += 1;
    
    	SoundManager.Instance.StopTimer();
    	SoundManager.Instance.Play("CameraShutter");
    	yield return new WaitForSeconds(0.3f);
    	// 연출을 위한 현재화면 캡처작업
    	Graphics.CopyTexture(camTexture, copyTexture);
    	// 플래시 연출
    	yield return playPanel.Flash();
    	SoundManager.Instance.Play("Success");
    	yield return new WaitForSeconds(0.3f);
    	SoundManager.Instance.Play("Yoohoo");
    	// 폭죽 파티클을 발생시켜준다.
    	foreach(GameObject particle in celebrateParticles)
    	{
    		particle.SetActive(true);
    	}
    	// 떨어지는 오브젝트 파티클을 생성해준다.
    	Instantiate(clearParticle, particleTransform.position, clearParticle.transform.rotation);
    	yield return new WaitForSeconds(1f);
    	// 클리어패널을 보여준다.
    	yield return clearPanel.ShowPanel();
    	currentUI = CurrentUI.CLEAR;
    }
    ```
    
    - 결과
      
        ![ezgif com-video-to-gif_(2)](https://github.com/user-attachments/assets/c451c2d1-f78e-4e1c-91c5-30d03ce63b12)

        

### 🎉 **파티클**

- 오브젝트 파티클을 만들기 위해서 파티클 쉐이더를 만들고. 그대로 파티클에 적용하면
  
    <img width="528" height="663" alt="image" src="https://github.com/user-attachments/assets/eabe1cfd-d8eb-4446-b863-cae9e11e2769" />

    
- 가로 세로의 비율차이가 많이 나는 경우(길쭉한 경우) 마음대로 비율이 바뀐 파티클이 만들어진다.
- 근본적인 해결방법을 찾지 못해서 비율이 바뀐 파티클은 임의로 조정해서 이 문제를 해결했다.
  
    <img width="531" height="660" alt="image" src="https://github.com/user-attachments/assets/206dcb4b-b6a0-4602-bdc2-000e32117ee0" />

    

## 💡새롭게 배운 내용

- 애니메이션으로 UI를 이동할 때 밑에서 올라오면서 나오고 위로 사라진 이후 다시 아래에서 올라오려고 할 때 아래와 같이 구현하면 위에 올라가 있던 UI가 아래로 내려가는 과정이 생긴다.
  
    <img width="375" height="98" alt="image" src="https://github.com/user-attachments/assets/5d568d9f-71a8-4103-8a6c-0277d4a1dd82" />

    
    이 문제를 해결하기 위해 아무것도 없는 New State로 돌아간 이후 다시 Show 애니메이션을 실행하면 정상적으로 작동하는 것을 알 수 있었다.
  
    <img width="427" height="172" alt="image" src="https://github.com/user-attachments/assets/856839b1-fc1b-48d9-ab07-bb89751ff050" />

    
- ParticleSystem을 이용해서 Sprite 처럼 만들어내는 법을 배웠다.
