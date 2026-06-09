<!-- App.vue - Componente Inicial com navegação -->
<
<template>
  <div class="min-h-screen bg-base-200 p-4">
    <div class="container mx-auto max-w-4xl">
      <!-- Header -->
      <div class="text-3xl text-center">
        <span>
          O aplicativo perfeito para
          <span class="text-rotate">
            <span>
              <span class="bg-teal-400 text-teal-800">ORGANIZAR</span>
              <span class="bg-red-400 text-red-800">PLANEJAR</span>
              <span class="bg-blue-400 text-blue-800">OTIMIZAR</span>
            </span>
          </span>
        </span>
      </div>
      <br/>
      <!-- Menu Principal -->
      <div v-if="telaAtual === 'menu'" class="grid md:grid-cols-2 gap-6">
        <!-- Opção 1: Texto -->
        <div @click="navegarPara('texto')"
             class="card bg-base-100">
          <div class="card-body items-center text-center">
            <div class="w-16 h-16 bg-primary/20 rounded-full flex items-center justify-center mb-4">
              <DocumentTextIcon class="size-6 text-primary" />
            </div>
            <h2 class="card-title text-2xl">Colar Texto</h2>
            <p class="text-base-content/70">
              Cole o texto das palestras no formato <br>
              <code class="badge badge-ghost">"Título 60min"</code> ou
              <code class="badge badge-ghost">"Título relâmpago"</code>
            </p>
            <div class="card-actions mt-4">
              <button class="btn btn-primary">Começar</button>
            </div>
          </div>
        </div>

        <!-- Opção 2: Criar Lista -->
        <div @click="navegarPara('lista')"
             class="card bg-base-100">
          <div class="card-body items-center text-center">
            <div class="w-16 h-16 bg-secondary/20 rounded-full flex items-center justify-center mb-4">
              <NumberedListIcon class="size-6 text-secondary" />
            </div>
            <h2 class="card-title text-2xl">Criar Lista</h2>
            <p class="text-base-content/70">
              Adicione palestras uma por uma com <br>
              nome, tempo e opção de relâmpago
            </p>
            <div class="card-actions mt-4">
              <button class="btn btn-secondary">Começar</button>
            </div>
          </div>
        </div>
      </div>

      <!-- Componente de Texto -->
      <TextoInput v-else-if="telaAtual === 'texto'"
                  @voltar="telaAtual = 'menu'"
                  @resultado="mostrarResultado" />

      <!-- Componente de Lista -->
      <ListaInput v-else-if="telaAtual === 'lista'"
                  @voltar="telaAtual = 'menu'"
                  @resultado="mostrarResultado" />

      <!-- Resultado -->
      <ResultadoView v-else-if="telaAtual === 'resultado'"
                     :dados="resultado"
                     @voltar="telaAtual = 'menu'"
                     @nova-entrada="telaAtual = $event" />
    </div>
  </div>
</template>

<script setup>import { ref } from 'vue'
import TextoInput from './components/TextoInput.vue'
import ListaInput from './components/ListaInput.vue'
import ResultadoView from './components/ResultadoView.vue'
import { DocumentTextIcon, NumberedListIcon } from '@heroicons/vue/24/solid'

const telaAtual = ref('menu')
const resultado = ref(null)

const navegarPara = (tela) => {
  telaAtual.value = tela
}

const mostrarResultado = (dados) => {
  resultado.value = dados
  telaAtual.value = 'resultado'
}</script>
