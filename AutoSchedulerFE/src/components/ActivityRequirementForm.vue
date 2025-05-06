<script setup lang="ts">
import type { ActivityRequirements } from '@/classes/activity';
import type { Group } from '@/classes/group';
import { useGroupStore } from '@/stores/groupStore';
import { storeToRefs } from 'pinia';
import { onMounted, reactive, ref, type Reactive, type Ref } from 'vue';

const store = useGroupStore();
const { groups, current } = storeToRefs(store)

onMounted(()=>{
    store.getGroupsForOrganization(1);
})

const newRequirement:Ref<ActivityRequirements> = ref({
    id: 0,
    activity: {id:0, title:"", organizationId:0, description:""},
    group: {id:0, organizationId:0, name:"", description:undefined, subGroups:[], requirements:[]},
    duration: 0,
    hallsize: undefined,
    halltype: undefined,
    timesPerWeek: undefined
}); 
</script>

<template>
    <form @submit="console.log(newRequirement)">
        <input type="number" v-model="newRequirement.duration" placeholder="Duration"/>
        <input type="number" v-model="newRequirement.hallsize" required="false" placeholder="Duration"/>
        <input type="number" v-model="newRequirement.halltype" required="false" placeholder="Duration"/>
        <input type="number" v-model="newRequirement.timesPerWeek" placeholder="Duration"/>
        <select v-model="newRequirement.group" id="">
            <option v-for="group in groups" :value="group">{{ group.name }}</option>
        </select>
        <button type="submit" @click.stop>Add</button>
    </form>
</template>