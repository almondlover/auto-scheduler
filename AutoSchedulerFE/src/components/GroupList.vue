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

const store = useGroupStore();
const { groups, current, currentOrganizationIdx } = storeToRefs(store);

onMounted(()=>{
    store.getGroupsForOrganization(currentOrganizationIdx.value);
});

watch(currentOrganizationIdx, ()=>{
    store.getGroupsForOrganization(currentOrganizationIdx.value);
});
</script>

<template>
    <ul>
        <li v-for="group in groups">
            <Card class="bg-light">
                <CardHeader>
                    <CardTitle class="text-center">
                        {{group.name}}
                    </CardTitle>
                    <CardDescription></CardDescription>
                </CardHeader>
                <CardContent class="flex flex-col gap-5">
                    <div>
                        {{ group.description }}
                    </div>
                    <div>
                        SubGroups:
                        <RouterLink :to="`/groups/${group.id}`" v-for="subGroup in group.subGroups">{{subGroup.name+" "}} </RouterLink>
                    </div>
                    <div class="flex gap-3 h-5 items-center justify-between">
                        <RouterLink class="underline" :to="`/groups/${group.id}`">Page</RouterLink>
                        <RouterLink class="underline" :to="`/timesheets/create`" @click="current=group.id">Create timesheet</RouterLink>
                        <Button @click="store.removeGroup(group.id)">Delete</Button>
                    </div>
                </CardContent>
            </Card>
        </li>
    </ul>
</template>