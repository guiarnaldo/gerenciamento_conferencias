<
<template>
  <div class="card bg-base-100 shadow-xl">
    <div class="card-body">
      <!-- Header -->
      <div class="flex items-center justify-between mb-6">
        <h2 class="card-title text-2xl flex items-center gap-2">
          Criar Lista de Palestras
        </h2>
        <button @click="$emit('voltar')" class="btn btn-ghost btn-sm">
          ← Voltar
        </button>
      </div>

      <!-- Formulário de Adição -->
      <div class="bg-base-200 p-4 rounded-xl mb-4">
        <div class="grid grid-cols-1 md:grid-cols-12 gap-3">
          <div class="md:col-span-6">
            <input v-model="novaPalestra.nome"
                   type="text"
                   placeholder="Nome da palestra"
                   class="input input-bordered w-full"
                   @keyup.enter="adicionar">
          </div>
          <div class="md:col-span-3">
            <input v-model.number="novaPalestra.tempo"
                   type="number"
                   placeholder="Minutos"
                   class="input input-bordered w-full"
                   :disabled="novaPalestra.isRelampago"
                   :class="{ 'input-disabled': novaPalestra.isRelampago }"
                   min="1"
                   @keyup.enter="adicionar">
          </div>
          <div class="md:col-span-3 flex items-center gap-2">
            <label class="label cursor-pointer gap-2 flex-1 justify-center">
              <input v-model="novaPalestra.isRelampago"
                     type="checkbox"
                     class="checkbox checkbox-primary">
              <span class="label-text">Relâmpago</span>
            </label>
            <button @click="adicionar"
                    class="btn btn-primary btn-circle btn-sm"
                    :disabled="!podeAdicionar">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
              </svg>
            </button>
          </div>
        </div>
      </div>

      <!-- Lista de Palestras -->
      <div class="space-y-2 max-h-96 overflow-y-auto">
        <div v-for="(palestra, index) in palestras"
             :key="index"
             class="flex items-center gap-3 p-3 bg-base-200 rounded-lg hover:bg-base-300 transition-colors">
          <div class="flex-1">
            <div class="font-semibold">
              {{ palestra.nome }}
              <span v-if="!palestra.isRelampago" class="badge badge-ghost">{{ `${palestra.tempo} min` }}</span>
              <span v-if="palestra.isRelampago" class="badge badge-accent badge-sm"><BoltIcon class="size-4" /></span>
            </div>
            <div class="text-sm text-base-content/70">

            </div>
          </div>
          <button @click="remover(index)"
                  class="btn btn-ghost btn-circle btn-sm text-error hover:bg-error/20">
          </button>
        </div>

        <!-- Empty State -->
        <div v-if="palestras.length === 0" class="text-center py-8 text-base-content/50">
          <NumberedListIcon class="size-10 text-ghost mx-auto" />
          <p>Nenhuma palestra adicionada</p>
          <p class="text-sm">Adicione palestras usando o formulário acima</p>
        </div>
      </div>

      <!-- Resumo -->
      <div v-if="palestras.length > 0" class="stats shadow mt-4 bg-base-200">
        <div class="stat place-items-center">
          <div class="stat-title">Total</div>
          <div class="stat-value text-primary">{{ palestras.length }}</div>
          <div class="stat-desc">palestras</div>
        </div>
        <div class="stat place-items-center">
          <div class="stat-title">Tempo Total</div>
          <div class="stat-value text-secondary">{{ tempoTotal }}min</div>
          <div class="stat-desc">{{ Math.floor(tempoTotal / 60) }}h {{ tempoTotal % 60 }}min</div>
        </div>
        <div class="stat place-items-center">
          <div class="stat-title">Relâmpagos</div>
          <div class="stat-value text-accent">{{ relampagosCount }}</div>
          <div class="stat-desc">5min cada</div>
        </div>
      </div>

      <!-- Ações -->
      <div class="flex gap-3 mt-4" v-if="palestras.length > 0">
        <button @click="organizar"
                class="btn btn-secondary flex-1"
                :disabled="carregando"
                :class="{ 'loading': carregando }">
          <NumberedListIcon class="size-6" />
          Organizar Conferência
        </button>
        <button @click="limparTudo"
                class="btn btn-ghost btn-error">
          Limpar
        </button>
      </div>

      <!-- Erro -->
      <div v-if="erro" class="alert alert-error mt-4">
        <ExclamationCircleIcon class="size-6" />
        <span>{{ erro }}</span>
      </div>
    </div>
  </div>
</template>

<script setup>import { ref, computed } from 'vue'

import { ExclamationCircleIcon, NumberedListIcon, BoltIcon } from '@heroicons/vue/24/solid'

const emit = defineEmits(['voltar', 'resultado'])

const palestras = ref([])
const novaPalestra = ref({
  nome: '',
  tempo: null,
  isRelampago: false
})
const carregando = ref(false)
const erro = ref('')

const podeAdicionar = computed(() => {
  if (!novaPalestra.value.nome.trim()) return false
  if (novaPalestra.value.isRelampago) return true
  return novaPalestra.value.tempo && novaPalestra.value.tempo > 0
})

const tempoTotal = computed(() =>
  palestras.value.reduce((acc, p) => acc + (p.isRelampago ? 5 : p.tempo), 0)
)

const relampagosCount = computed(() =>
  palestras.value.filter(p => p.isRelampago).length
)

const adicionar = () => {
  if (!podeAdicionar.value) return

  palestras.value.push({
    nome: novaPalestra.value.nome.trim(),
    tempo: novaPalestra.value.isRelampago ? 5 : novaPalestra.value.tempo,
    isRelampago: novaPalestra.value.isRelampago
  })

  novaPalestra.value = { nome: '', tempo: null, isRelampago: false }
}

const remover = (index) => {
  palestras.value.splice(index, 1)
}

const limparTudo = () => {
  palestras.value = []
}

const organizar = async () => {
  carregando.value = true
  erro.value = ''

  try {
    const response = await fetch(`api/conferencia/organizar-json`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ palestras: palestras.value })
    })

    const dados = await response.json()

    if (!response.ok) {
      throw new Error(dados.erro || 'Erro na requisição')
    }

    emit('resultado', {
      tipo: 'conferencia',
      ...dados
    })
  } catch (err) {
    erro.value = err.message
  } finally {
    carregando.value = false
  }
}</script>
