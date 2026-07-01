pipeline {
    agent any
    stages {
        stage('Restore') {
            steps {
                bat 'dotnet restore'
            }
        }
        stage('Build') {
            steps {
                bat 'dotnet build'
            }
        }
        stage('Test') {
            steps {
                bat 'dotnet test --logger "trx;LogFileName=test_results.trx" --results-directory TestResults'
            }
            post {
                always {
                    mstest testResultsFile: 'TestResults/*.trx'
                }
            }
        }
        stage('SonarQube Analysis') {
            steps {
                withSonarQubeEnv('SonarQube') {
                    bat 'dotnet sonarscanner begin /k:"jenkins_deploy" /d:sonar.host.url=%SONAR_HOST_URL% /d:sonar.login=%SONAR_AUTH_TOKEN%'
                    bat 'dotnet build'
                    bat 'dotnet sonarscanner end /d:sonar.login=%SONAR_AUTH_TOKEN%'
                }
            }
        }
        stage('Quality Gate') {
            steps {
                timeout(time: 5, unit: 'MINUTES') {
                    waitForQualityGate abortPipeline: false
                }
            }
        }
        stage('Deploy') {
            steps {
                script {
                    bat 'dotnet publish -c Release -o publish_output'
                    if (env.BRANCH_NAME == 'Dev') {
                        bat 'xcopy /E /Y /I publish_output F:\\Project\\Jenkins_deployment\\Dev'
                    } else if (env.BRANCH_NAME == 'QA') {
                        bat 'xcopy /E /Y /I publish_output F:\\Project\\Jenkins_deployment\\QA'
                    } else if (env.BRANCH_NAME == 'main') {
                        bat 'xcopy /E /Y /I publish_output F:\\Project\\Jenkins_deployment\\Main'
                    }
                }
            }
        }
    }
}