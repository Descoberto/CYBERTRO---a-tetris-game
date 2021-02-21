﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class gameManager : MonoBehaviour {

    // Attributes -
    public static short altura = 20;
    public static short largura = 10;
    public short score = 0;
    public Text textoscore;
    public Text highScore;
    public short pontoDificuldade;
    public float dificuldade = 1;
    public bool pause = false;

    public static Transform[,] grade = new Transform[largura, altura]; // <-- Instancia de um Array bidimensional.

    // Initialization -
    void Start ()
    {
        highScore.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
    }

    // Update -
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameOver();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            pausa();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if ((score & PlayerPrefs.GetInt("HighScore", 0)) == 0)
            {
                Reset();
            }
                SceneManager.LoadScene("gamePlay");
        }

        textoscore.text = score.ToString();
        if (score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", score);
            highScore.text = score.ToString();
        }
    }

    // Methods -
    public void Reset()
    {
        PlayerPrefs.DeleteAll();
        highScore.text = "0";
    } // <-- Secret.

    public void pausa()
    {
        if (pause)
        {
            pause = false;
        }
        else
        {
            pause = true;
        }
    } // <-- Pause the game

    public bool dentroGrade(Vector2 posicao)
    {
        return ((int)posicao.x >= 0 && (int)posicao.x < largura && (int)posicao.y >= 0);
    } // <-- Faz a verificação se as peças estão dentro ou fora da grade.

    public Vector2 arredonda(Vector2 nA)
    {
        return new Vector2(Mathf.Round(nA.x), Mathf.Round(nA.y));
    } // <-- Método que arredonda um número real para um inteiro.

    public void atualizaGrade(tetroMov pecaTetris)
    {
        for (int y=0; y < altura; y++)
        {
            for (int x=0; x < largura; x++)
            {
                if (grade [x, y] != null)
                {
                    if (grade[x, y].parent == pecaTetris.transform)
                    {
                        grade[x, y] = null;
                    }
                }
            }
        }
        foreach (Transform peca in pecaTetris.transform)
        {
            Vector2 posicao = arredonda(peca.position);

            if (posicao.y < altura)
            {
                grade[(int)posicao.x, (int)posicao.y] = peca;
            }
        }
    } // <-- Método que atualiza a Grade quando sofrer alterações.

    public Transform posicaoTransformGrade(Vector2 posicao)
    {
        if (posicao.y > altura - 1)
        {
            return null;
        }
        else
        {
            return grade[(int)posicao.x, (int)posicao.y];
        }
    }  // <-- Método que pega os valores da posição e verifica se são validos.

    public bool linhaCheia(int y)
    {
        for (int x = 0; x < largura; x++)
        {
            if (grade[x,y] == null)
            {
                return false;
            }
        }
        return true;
    } // <-- Verifica se a linha está cheia.

    public void deletaQuadrado (int y)
    {
        for (int x = 0; x < largura; x++)
        {
            Destroy(grade[x, y].gameObject);

            grade[x, y] = null;
        }
    } // <-- Delete the line with the completed squares.

    public void moveLinhaBaixo(int y)
    {
        for (int x = 0; x < largura; x++)
        {
            if (grade[x, y] != null)
            {
                grade[x, y - 1] = grade[x, y];
                grade[x, y] = null;

                grade[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    } // <-- Método que move a linha que sobrou para baixo.

    public void moveTodasLinhasBaixo(int y)
    {
        for (int i = y; i < altura; i++)
        {
            moveLinhaBaixo(i);
        }
    } // <-- Método que move todas as linhas que sobraram para baixo.

    public void apagaLinha()
    {
        for (int y = 0; y < altura; y++)
        {
            if (linhaCheia(y))
            {
                deletaQuadrado(y);
                moveTodasLinhasBaixo(y + 1);
                y--;
                score += 100;
                pontoDificuldade += 100;
            }
        }
    }  // <-- Método que apaga a linha.

    public bool acimaGrade(tetroMov pecaTetroMino)
    {
        for (int x = 0; x < largura; x++)
        {
            foreach(Transform quadrado in pecaTetroMino.transform)
            {
                Vector2 posicao = arredonda(quadrado.position);
                if (posicao.y > altura - 1)
                {
                    return true;
                }
            }
        }
        return false;
    } // <-- Método que verifica se está acima da grade.

    public void gameOver()
    {
        SceneManager.LoadScene("gameOver");
    } // <-- Método que dá GameOver.
}
