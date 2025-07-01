<script setup lang="ts">
import type { Group } from '@/classes/group';
import { useGroupStore } from '@/stores/groupStore';
import { storeToRefs } from 'pinia';
import { onMounted, watch } from 'vue';
import Button from './ui/button/Button.vue';
import Card from './ui/card/Card.vue';
import CardTitle from './ui/card/CardTitle.vue';
import CardContent from './ui/card/CardContent.vue';
import CardDescription from './ui/card/CardDescription.vue';
import CardHeader from './ui/card/CardHeader.vue';
import { useRoute } from 'vue-router';

const store = useGroupStore();
const { groups, current, currentOrganizationIdx } = storeToRefs(store);
const route = useRoute();

onMounted(()=>{
    current.value=parseInt(route.params.id.toString());
    console.log(current.value);
    store.getGroup(current.value);
});

watch(route, ()=>{
    current.value=parseInt(route.params.id.toString());
    console.log(current.value);
    store.getGroup(current.value);
},
{deep:true});
</script>

<template>
    <Card class="bg-secondary m-5">
        <CardHeader>
            <CardTitle class="text-center text-lg font-bold">
                {{store.currentGroup?.name}}
            </CardTitle>
            <CardDescription>{{ store.currentGroup?.description }}</CardDescription>
        </CardHeader>
        <CardContent class="flex flex-col gap-5">
            <div>
                SubGroups:
                <RouterLink class="hover:underline" :to="`/groups/${subGroup.id}`" v-for="subGroup in store.currentGroup?.subGroups">{{subGroup.name+" "}} </RouterLink>
            </div>
            <div class="flex gap-3 h-5 items-center justify-between">
                <RouterLink class="underline" :to="`/timesheets/create`">Create timesheet</RouterLink>
                <Button @click="store.removeGroup(store.currentGroup?.id??-1)">Delete</Button>
            </div>
        </CardContent>
    </Card>
</template>