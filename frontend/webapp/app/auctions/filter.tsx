import { useParamsStore } from "@/hooks/useParamsStore";
import { Button } from "flowbite-react";
import React from "react";

const pageSizeOptions = [4, 8, 12];

const Filter = () => {
  const pageSize = useParamsStore((state) => state.pageSize);
  const setParams = useParamsStore((state) => state.setParams);

  return (
    <div className="flex justify-between items-center mb-4">
      <div>
        <span className="uppercase text-sm text-gray-500 mr-2">Page size</span>
        <Button.Group>
          {pageSizeOptions.map((value, i) => (
            <Button
              key={i}
              onClick={() => {
                setParams({ pageSize: value });
              }}
              color={`${pageSize === value ? "red" : "gray"}`}
              className="focus:ring-0 focus:text-lightRed">
              {value}
            </Button>
          ))}
        </Button.Group>
      </div>
    </div>
  );
};

export default Filter;