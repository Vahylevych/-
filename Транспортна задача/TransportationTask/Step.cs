using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TransportationTask
{
    /*    Клас для работи з кроком розвязання, 
     *  кожний крок відповідає  
     *  одному плану розвязання,            */
    class Step
    {
        /* Кількість  постачальників, */
        int n = 0;
        /* Кількість  замовників, */
        int m = 0;
        /* План розвязання, */
        List<List<int>> plan = new List<List<int>>();
        /* Матриця  вартостей, */
        List<List<int>> a = new List<List<int>>();
        /* Вектор потенціалів постачальників, */
        List<int> u = new List<int>();
        /* Вектор потенціалів  замовників,  */
        List<int> v = new List<int>();
        /* Матриця потенціалів, */
        List<List<int>> c = new List<List<int>>();
        
         /* Конструктор кроку, */
        public Step(List<List<int>> a1, List<List<int>> plan1) 
        {
            a = CopyElement.CopyMatrix(a1);
            plan = CopyElement.CopyMatrix(plan1);
            n = plan.Count;
            m = plan[0].Count;
            CreatePotentials();
            CreateMatrixPotentials();
        }
        
        /* Вартість плану розвязання, */
        public int FunctionCost()
        {
            int a1 = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (plan[i][j] > 0)
                    {
                        a1 += plan[i][j] * a[i][j];
                    }
                }
            }
            return a1;
        }

        /* Функція перевіряє, оптимальність розвязання, */
        public bool IsOptimal()
        {
            bool b = true;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (c[i][j] >0)
                    {
                        b = false;
                        break;
                    }
                }
                if (b == false)
                {
                    break;
                }
            }
            return b;
        }

         /* Індекси  елементу, який буде введений в базис, */
        public Tuple<int, int> MaxPotential()
        {
            int i1 = 0;
            int j1 = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (c[i][j] > 0 &&
                        (c[i][j] > c[i1][j1] || 
                        c[i][j] == c[i1][j1] && a[i][j] < a[i1][j1]))
                    {
                        i1 = i;
                        j1 = j;
                    }
                }
            }
            return Tuple.Create<int, int>(i1, j1);
        }

         /* Вивід всієї інформації про крок на форму, */
        public void OutStepToDataGridView(DataGridView dataGridA,
                        DataGridView dataGridB,TextBox textBox1)
        {
            textBox1.Text = FunctionCost().ToString();
            OutPlanToDataGridView(dataGridA);
            OutPotentialsToDataGridView(dataGridB);
        }

         /*   Вивід матриці з додатніми елементами,
          * протилежний  елементам  побудованої матриці 
          * потенціалів  на форму,                         */
        private void OutPotentialsToDataGridView(
                                       DataGridView dataGridA)
        {
            dataGridA.RowCount = n;
            dataGridA.ColumnCount = m;
            int i1 = (n % 2 == 0) ? (n / 2 - 1) : ((n - 1) / 2);
            dataGridA.Rows[i1].HeaderCell.Value = " E = ";
            for (int i = 0; i < m; i++)
            {
                dataGridA.Columns[i].Width = 58;
            }
            for (int i = 0; i < n; i++)
            {
                dataGridA.Rows[i].Height = 40;
                for (int j = 0; j < m; j++)
                {
                    dataGridA[j, i].Value = -c[i][j];
                }
            }
        }

         /*    Вивід плану розвязання потенціалів на  форму,      */
        private void OutPlanToDataGridView(DataGridView dataGridA)
        {
            dataGridA.RowCount = n+1;
            dataGridA.ColumnCount = m+1;
            for (int i = 0; i < n; i++)
            {
                dataGridA[m,i].Value =
                    string.Format("u[{0}]={1},", i + 1, u[i]);
            }
            for (int i = 0; i < m; i++)
            {
                dataGridA.Columns[i].Width = 58;
                dataGridA[i,n].Value=
                    string.Format("v[{0}]={1},", i + 1, v[i]);
            }
            dataGridA.Columns[m].Width = 80;
            dataGridA.Rows[n].Height = 40;
            for (int i = 0; i < n; i++)
            {
                dataGridA.Rows[i].Height = 40;
                for (int j = 0; j < m; j++)
                {
                  dataGridA[j, i].Value =
                    plan[i][j] > -1 ? plan[i][j].ToString() : "-";
                }
            }
        }


        /*  Функція знаходить потенціали  u и v,  */
        private void CreatePotentials()
        {
            List<int> u1 = new List<int>();
            List<int> v1 = new List<int>();
            List<int> i1 = new List<int>();
            List<int> j1 = new List<int>();
            int s = 1;
            int k = n + m - 1;
            u = new List<int>();
            v = new List<int>();
            for (int i = 0; i < n; i++)
            {
                u.Add(0);
                u1.Add(0);
            }
            for (int i = 0; i < m; i++)
            {
                v.Add(0);
                v1.Add(0);
            }
            i1.Add(0);
            u[i1[0]] = 0;
            u1[i1[0]] = 1;
            for (int i = 0; i < k; i++)
            {
                if (s == 1)
                {
                    j1 = new List<int>();
                    for (int j = 0; j < i1.Count; j++)
                    {
                        for (int w = 0; w < m; w++)
                        {
                            if (plan[i1[j]][w] > -1 && v1[w] != 1)
                            {
                                v[w] = a[i1[j]][w] - u[i1[j]];
                                v1[w] = 1;
                                j1.Add(w);
                            }
                        }
                    }
                    s = 2;
                }
                else
                {
                    i1 = new List<int>();
                    for (int j = 0; j < j1.Count; j++)
                    {
                        for (int w = 0; w < n; w++)
                        {
                            if (plan[w][j1[j]] > -1 && u[w] != 1)
                            {
                                u[w] = a[w][j1[j]] - v[j1[j]];
                                u1[w] = 1;
                                i1.Add(w);
                            }
                        }
                    }
                    s = 1;
                }
            }
        }

         /*  Функція  створює матрицю потенціалів,  */
        private void CreateMatrixPotentials()
        {
            List<int> a1 = new List<int>();
            c = new List<List<int>>();
            for (int i = 0; i < n; i++)
            {
                a1 = new List<int>();
                for (int j = 0; j < m; j++)
                {
                    a1.Add(u[i] + v[j]-a[i][j]);
                }
                c.Add(a1);
            }
        }


    }
}
