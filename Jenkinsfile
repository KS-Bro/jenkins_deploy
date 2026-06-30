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
                bat 'dotnet build --configuration Release'
            }
        }

        stage('Test') {
            when {
                anyOf {
                    branch 'QA'
                    branch 'main'
                }
            }
            steps {
                bat 'dotnet test'
            }
        }

        stage('Publish') {
            when {
                branch 'main'
            }
            steps {
                bat 'dotnet publish -c Release -o publish'
            }
        }
    }
}